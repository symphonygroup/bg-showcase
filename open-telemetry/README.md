# Open Telemetry - AWS vs OSS (Grafana)

In this PoC we explored two options for enabling observability for our solution. 

This solution contains of 2 .NET services that communicate to each other through both HTTP calls and events. For events we used MassTransit library with SQS+SNS as a transport layer for events. This service stack will be used for both PoCs.

Technology stack of services and event bus is not important for this PoC. Different parts of these two options could also be combined as we will explore different options for deploying and using OTel collector (for collecting traces & metrics):

- AWS PoC will focus on using OTel collector using side-car strategy, auto-instrumentation & AWS CloudWatch observability solutions
- OSS PoC will focus on using OTel collector as a standalone service, in-code instrumentation & Grafana+Prometheus solutions for observability

In order to run samples, you will need to set up AWS CLI, API Keys for it and `aws configure`. 

## AWS w/ Auto Instrumentation

This PoC will create a AWS CloudFormation Stacks for services (ECS), database (RDS) and side-car for each ECS service with CloudWatch agent that will send telemetry data and traces to AWS CloudWatch.

### Enable Application Signals

Follow the instructions in this page to enable Application Signals. This step is required as it will create a role that will be used later in the process.
https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/CloudWatch-Application-Signals-ECS-Sidecar.html#CloudWatch-Application-Signals-ECS-Grant

### Build and push Docker images

Go to base folder and run `docker compose build` in order to build containers for 2 service.

Create two ECR repositories:
- signals/webapi
- signals/consumer

Tag & push to ECR the containers you built. There is a how-to `View push command` in AWS ECR.

### Deploy Stacks

Go to root folder and setup AWS CDK as explained here https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html
You might need to set AWS credentials in the terminal where you will be running commands. 

Run `cdk bootstrap` to set it correctly on AWS account. Then run first `cdk synth` and then `cdk deploy --all` and follow the process as you might need to allow some actions. This process can take some time as it creates RDS & Load Balancer that can take some time.

IMPORTANT: This stack uses ECS, RDS, LoadBalancer and other AWS services and is not covered with Free Tier.

### Run k6

Install `k6` on your system. Follow the instructions here https://grafana.com/docs/k6/latest/set-up/install-k6/

Check `/src/k6/test.js` file. Change the base URL to the one of your Load Balancer (it was printed out as the output of the `cdk deploy` command you ran in a previous step).

Run tests with `k6 run test.js` in order to create some traffic.

### Explore Traces, dashboards...

Go to AWS CloudWatch console and investigate Services and Service Map.
Check also X-Ray traces and Trace Map.

### Destroy AWS Resources

When done with exploring, we recommend that you destroy all resources with `cdk destroy --all` command. 

## OSS with Nuget packages

Make sure you set up all needed for AWS.

### Start observability stack

Go to `/src/grafana` folder and run `docker compose build` and then `docker compose up` in order to start observability stack.

### Start Services

Go to base folder and run `docker compose build` and then `./run-compose.sh` or `run-compose.bat` that will set up environment variables from default AWS profile and start docker compose. If set up is done correctly, you should see in Docker that both services are up, database was initialized and services are connected to SQS. There should not be errors in service's logs. 

### Run k6 Tests

Install `k6` on your system. Follow the instructions here https://grafana.com/docs/k6/latest/set-up/install-k6/

In order to create some traffic, go to `/src/k6` folder and run `k6 run test.js`. By default, it is calling `http://localhost:45080` where WebApi is exposed. Play with the duration of stages and other parameters to create more test data.

### Add ASP.NET Dashboards to Grafana

Go to `http://localhost:3000/` where Grafana UI is hosted. It already has predefined sources for Prometheus (metrics) & Tempo (tracing).

You can add some Dashboards for ASP.NET. See recommendations here https://github.com/dotnet/aspire/tree/main/src/Grafana

### Look at Tempo traces

You can Explore sources. We recommend to explore Tempo and select Query Type "Service Graph" in order to see some dependencies between services. In a graph you will see a service named "Signals" - that is a PostgreSQL DB.