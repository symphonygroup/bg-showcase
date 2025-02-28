using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.ServiceDiscovery;
using Constructs;
using HealthCheck = Amazon.CDK.AWS.ElasticLoadBalancingV2.HealthCheck;
using Volume = Amazon.CDK.AWS.ECS.Volume;

namespace AwsAppSignals
{
    public class EcsStack : Stack
    {
        private const string OpenTelemetryAutoInstrumentation = "opentelemetry-auto-instrumentation";

        internal EcsStack(Construct scope, string id, NetworkingStack networkingStack, RdsStack rdsStack, IStackProps props = null) : base(scope, id, props)
        {
            // Create ECS Cluster
            var cluster = new Cluster(this, "EcsCluster", new ClusterProps
            {
                Vpc = networkingStack.Vpc
            });
            
            // Define IAM Role for ECS Task Execution
            var executionRole = new Role(this, "TaskExecutionRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com"),
                ManagedPolicies =
                [
                    ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"),
                    ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"), // Grants ECR pull access
                ]
            });
            
            var ecsTaskRole = new Role(this, "EcsTaskRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com"),
                ManagedPolicies =
                [
                    ManagedPolicy.FromAwsManagedPolicyName("CloudWatchAgentServerPolicy")
                ]
            });
            // Attach AWS Managed Policies for SQS and SNS (for MassTransit)
            ecsTaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonSQSFullAccess"));
            ecsTaskRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonSNSFullAccess"));
            
            var webApiRepository = Repository.FromRepositoryName(this, "signals/webapi", "signals/webapi");
            
            var dnsNamespace = new PrivateDnsNamespace(this, "DnsNamespace", new PrivateDnsNamespaceProps {
                Name = "signals", // Custom domain name
                Vpc = networkingStack.Vpc
            });

            // API Service - Public with Load Balancer
            const string apiName = "api";
            var apiSecurityGroup = SecurityGroup.FromSecurityGroupId(this, "ImportedApiSecurityGroup",
                networkingStack.ApiSecurityGroup.SecurityGroupId);
            var apiTaskDefinition = new FargateTaskDefinition(this, "ApiTask", new FargateTaskDefinitionProps
            {
                MemoryLimitMiB = 512,
                Cpu = 256,
                ExecutionRole = executionRole,
                TaskRole = ecsTaskRole,
                Volumes = [new Volume {Name = OpenTelemetryAutoInstrumentation}]
            });
            
            AddCloudWatchAgent(apiTaskDefinition, apiName);
            var apiInitContainer = AddInitContainer(apiTaskDefinition);

            var apiContainer = apiTaskDefinition.AddContainer("ApiContainer", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromEcrRepository(webApiRepository),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps { StreamPrefix = apiName }),
                Essential = true,
                Environment = new Dictionary<string, string>
                {
                    { "ConnectionStrings__DefaultConnection", rdsStack.ConnectionString },
                    { "OTEL_RESOURCE_ATTRIBUTES", $"service.name={apiName},service.namespace={dnsNamespace.NamespaceName}" },
                    { "CORECLR_ENABLE_PROFILING", "1" },
                    { "CORECLR_PROFILER", "{918728DD-259F-4A6A-AC2B-B85E1B658318}" },
                    { "CORECLR_PROFILER_PATH", "/otel-auto-instrumentation/linux-x64/OpenTelemetry.AutoInstrumentation.Native.so" },
                    { "DOTNET_ADDITIONAL_DEPS", "/otel-auto-instrumentation/AdditionalDeps" },
                    { "DOTNET_SHARED_STORE", "/otel-auto-instrumentation/store" },
                    { "DOTNET_STARTUP_HOOKS", "/otel-auto-instrumentation/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll" },
                    { "OTEL_DOTNET_AUTO_HOME", "/otel-auto-instrumentation" },
                    { "OTEL_DOTNET_AUTO_PLUGINS", "AWS.Distro.OpenTelemetry.AutoInstrumentation.Plugin, AWS.Distro.OpenTelemetry.AutoInstrumentation" },
                    { "OTEL_LOGS_EXPORTER", "none" },
                    { "OTEL_METRICS_EXPORTER", "none" },
                    { "OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf" },
                    { "OTEL_AWS_APPLICATION_SIGNALS_ENABLED", "true" },
                    { "OTEL_AWS_APPLICATION_SIGNALS_EXPORTER_ENDPOINT", "http://localhost:4316/v1/metrics" },
                    { "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", "http://localhost:4316/v1/traces" },
                    { "OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4316" },
                    { "OTEL_TRACES_SAMPLER", "xray" },
                    { "OTEL_TRACES_SAMPLER_ARG", "endpoint=http://localhost:2000" },
                    { "OTEL_PROPAGATORS", "tracecontext,baggage,b3,xray" }
                },
                PortMappings = [new PortMapping { ContainerPort = 8080 }]
            });
            apiContainer.AddMountPoints(new MountPoint
            {
                SourceVolume = OpenTelemetryAutoInstrumentation, // Replace with your volume name
                ContainerPath = "/otel-auto-instrumentation", // Replace with your container path
                ReadOnly = false
            });
            apiContainer.AddContainerDependencies(new ContainerDependency{ Container = apiInitContainer, Condition = ContainerDependencyCondition.SUCCESS});
            
            var apiService = new ApplicationLoadBalancedFargateService(this, "ApiService", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                DesiredCount = 1, // Single instance for cost efficiency
                TaskDefinition = apiTaskDefinition,
                PublicLoadBalancer = true,
                SecurityGroups = [apiSecurityGroup],
                CloudMapOptions = new CloudMapOptions {
                    CloudMapNamespace = dnsNamespace,
                    Name = apiName // DNS name for the service
                }
            });
            apiService.TargetGroup.ConfigureHealthCheck(new HealthCheck
            {
                Path = "/",
                Port = "8080"
            });
            
            // Worker Service - Private (No Load Balancer)
            var workerTaskDefinition = new FargateTaskDefinition(this, "WorkerTask", new FargateTaskDefinitionProps
            {
                MemoryLimitMiB = 512,
                Cpu = 256,
                ExecutionRole = executionRole,
                TaskRole = ecsTaskRole,
                Volumes = [new Volume {Name = OpenTelemetryAutoInstrumentation}]
            });
            
            const string workerName = "worker";
            
            AddCloudWatchAgent(workerTaskDefinition, workerName);
            var workerInitContainer = AddInitContainer(workerTaskDefinition);
            
            var consumerRepository = Repository.FromRepositoryName(this, "signals/consumer", "signals/consumer");
            
            var workerContainer = workerTaskDefinition.AddContainer("WorkerContainer", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromEcrRepository(consumerRepository),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps { StreamPrefix = workerName }),
                Environment = new Dictionary<string, string>
                {
                    { "WebApi__Url", $"http://{apiName}.{dnsNamespace.NamespaceName}:8080" },
                    { "OTEL_RESOURCE_ATTRIBUTES", $"service.name={workerName},service.namespace={dnsNamespace.NamespaceName}" },
                    { "CORECLR_ENABLE_PROFILING", "1" },
                    { "CORECLR_PROFILER", "{918728DD-259F-4A6A-AC2B-B85E1B658318}" },
                    { "CORECLR_PROFILER_PATH", "/otel-auto-instrumentation/linux-x64/OpenTelemetry.AutoInstrumentation.Native.so" },
                    { "DOTNET_ADDITIONAL_DEPS", "/otel-auto-instrumentation/AdditionalDeps" },
                    { "DOTNET_SHARED_STORE", "/otel-auto-instrumentation/store" },
                    { "DOTNET_STARTUP_HOOKS", "/otel-auto-instrumentation/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll" },
                    { "OTEL_DOTNET_AUTO_HOME", "/otel-auto-instrumentation" },
                    { "OTEL_DOTNET_AUTO_PLUGINS", "AWS.Distro.OpenTelemetry.AutoInstrumentation.Plugin, AWS.Distro.OpenTelemetry.AutoInstrumentation" },
                    { "OTEL_LOGS_EXPORTER", "none" },
                    { "OTEL_METRICS_EXPORTER", "none" },
                    { "OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf" },
                    { "OTEL_AWS_APPLICATION_SIGNALS_ENABLED", "true" },
                    { "OTEL_AWS_APPLICATION_SIGNALS_EXPORTER_ENDPOINT", "http://localhost:4316/v1/metrics" },
                    { "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", "http://localhost:4316/v1/traces" },
                    { "OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4316" },
                    { "OTEL_TRACES_SAMPLER", "xray" },
                    { "OTEL_TRACES_SAMPLER_ARG", "endpoint=http://localhost:2000" },
                    { "OTEL_PROPAGATORS", "tracecontext,baggage,b3,xray" }
                }
            });
            workerContainer.AddMountPoints(new MountPoint
            {
                SourceVolume = OpenTelemetryAutoInstrumentation, // Replace with your volume name
                ContainerPath = "/otel-auto-instrumentation", // Replace with your container path
                ReadOnly = false
            });
            workerContainer.AddContainerDependencies(new ContainerDependency{ Container = workerInitContainer, Condition = ContainerDependencyCondition.SUCCESS});
            
            var workerService = new FargateService(this, "WorkerService", new FargateServiceProps
            {
                Cluster = cluster,
                TaskDefinition = workerTaskDefinition,
                DesiredCount = 1,
                SecurityGroups = [networkingStack.WorkerSecurityGroup],
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
                CloudMapOptions = new CloudMapOptions {
                    CloudMapNamespace = dnsNamespace,
                    Name = workerName // DNS name for the service
                }
            });

            // Output
            new CfnOutput(this, "ApiServiceURL", new CfnOutputProps
            {
                Value = apiService.LoadBalancer.LoadBalancerDnsName
            });

            new CfnOutput(this, "DbEndpoint", new CfnOutputProps
            {
                Value = rdsStack.DatabaseInstance.DbInstanceEndpointAddress
            });
        }

        private void AddCloudWatchAgent(TaskDefinition taskDefinition, string logGroup)
        {
            // Add the CloudWatch Agent container to the task definition
            taskDefinition.AddContainer("ecs-cwagent", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry("public.ecr.aws/cloudwatch-agent/cloudwatch-agent:latest"), // Replace with your CloudWatch Agent image
                Environment = new Dictionary<string, string>
                {
                    { "CW_CONFIG_CONTENT", "{\"agent\": {\"debug\": true}, \"traces\": {\"traces_collected\": {\"application_signals\": {\"enabled\": true}}}, \"logs\": {\"metrics_collected\": {\"application_signals\": {\"enabled\": true}}}}" }
                },
                Essential = false,
                PortMappings = [
                    new PortMapping {ContainerPort = 4316, HostPort = 4316},
                    new PortMapping {ContainerPort = 2000, HostPort = 2000}
                ],
                Logging = new AwsLogDriver(new AwsLogDriverProps
                {
                    StreamPrefix = "ecs",
                    LogGroup = new LogGroup(this, $"CwAgentLogGroup-{logGroup}")
                })
            });
        }

        private ContainerDefinition AddInitContainer(TaskDefinition taskDefinition)
        {
            // Add the init container to the task definition
            var initContainer = taskDefinition.AddContainer("init", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry("public.ecr.aws/aws-observability/adot-autoinstrumentation-dotnet:v1.6.0"), // Replace with your init container image
                Essential = false,
                Command = ["cp", "-a", "autoinstrumentation/.", "/otel-auto-instrumentation"] // Replace with your init container command
            });
            
            // Add mount points to the init container
            initContainer.AddMountPoints(new MountPoint
            {
                SourceVolume = OpenTelemetryAutoInstrumentation, // Replace with your volume name
                ContainerPath = "/otel-auto-instrumentation", // Replace with your container path
                ReadOnly = false
            });
            
            return initContainer;
        }
    }
}
