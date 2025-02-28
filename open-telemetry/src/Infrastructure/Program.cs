using Amazon.CDK;

namespace AwsAppSignals
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var networking = new NetworkingStack(app, "SignalsNetworkingStack");
            var db = new RdsStack(app, "SignalsRdsStack", networking);
            var ecs = new EcsStack(app, "SignalsEcsStack", networking, db);
            app.Synth();
        }
    }
}
