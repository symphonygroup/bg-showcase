using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Constructs;
using InstanceType = Amazon.CDK.AWS.EC2.InstanceType;

namespace AwsAppSignals;

public class RdsStack : Stack
{
    public string ConnectionString { get; }
    public DatabaseInstance DatabaseInstance { get; }
    
    internal RdsStack(Construct scope, string id, NetworkingStack networkingStack, IStackProps props = null) 
        : base(scope, id, props)
    {
        // Create an RDS instance
        var dbUsername = "pocuser";
        var dbPassword = "pocpass123!"; // Replace with a securely stored password
        var dbName = "signals";
        
        // Security Group for RDS allowing only internal connections
        
            
        DatabaseInstance = new DatabaseInstance(this, "MyPostgresDB", new DatabaseInstanceProps
        {
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
            {
                Version = PostgresEngineVersion.VER_15
            }),
            InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
            Vpc = networkingStack.Vpc,
            VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
            Credentials = Credentials.FromPassword(dbUsername, SecretValue.UnsafePlainText(dbPassword)),
            MultiAz = false,
            AllocatedStorage = 20,
            StorageEncrypted = true,
            SecurityGroups = [networkingStack.DbSecurityGroup],
            PubliclyAccessible = false
        });
            
        ConnectionString = $"Host={DatabaseInstance.DbInstanceEndpointAddress};Port=5432;Database={dbName};Username={dbUsername};Password={dbPassword}";
    }
}