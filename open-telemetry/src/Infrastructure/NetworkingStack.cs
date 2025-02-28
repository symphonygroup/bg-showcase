using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.EKS;
using Constructs;

namespace AwsAppSignals;

public class NetworkingStack : Stack
{
    public Vpc Vpc { get; }

    public SecurityGroup DbSecurityGroup { get; }
    public SecurityGroup ApiSecurityGroup { get; }
    public SecurityGroup WorkerSecurityGroup { get; }

    internal NetworkingStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        // Create a VPC
        Vpc = new Vpc(this, "MyVpc", new VpcProps
        {
            MaxAzs = 2, // Need at least 2 for RDS,
            NatGateways = 1,
            SubnetConfiguration =
            [
                new SubnetConfiguration
                {
                    Name = "PublicSubnet",
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24
                },
                new SubnetConfiguration
                {
                    Name = "IsolatedSubnet",
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS, // No NAT Gateway required
                    CidrMask = 24
                }
            ]
        });
        
        // Security Group for Worker Service (Private)
        WorkerSecurityGroup = new SecurityGroup(this, "WorkerSecurityGroup", new SecurityGroupProps
        {
            Vpc = Vpc,
            Description = "Allow only internal communication",
            AllowAllOutbound = true
        });
        
        // Security Group for API service
        ApiSecurityGroup = new SecurityGroup(this, "ApiSecurityGroup", new SecurityGroupProps
        {
            Vpc = Vpc,
            Description = "Allow HTTP/HTTPS from outside",
            AllowAllOutbound = true
        });
        ApiSecurityGroup.AddIngressRule(Peer.Ipv4(Vpc.VpcCidrBlock), Port.Tcp(8080), "Allow traffic from load balancer");
        ApiSecurityGroup.AddIngressRule(WorkerSecurityGroup, Port.Tcp(8080), "Allow traffic from Worker");
        
        // Security Group for RDS
        DbSecurityGroup = new SecurityGroup(this, "DbSecurityGroup", new SecurityGroupProps
        {
            Vpc = Vpc,
            Description = "Allow internal access to PostgreSQL",
            AllowAllOutbound = true
        });
        DbSecurityGroup.AddIngressRule(
            peer: ApiSecurityGroup,
            connection: Port.Tcp(5432),
            description: "Allow access to PostgreSQL from API Security Group"
        );
    }
}