using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Tools.Singleton;
using Akka.Hosting;
using Akka.Logger.Serilog;
using Akka.Remote.Hosting;
using Akka.Skeleton.Cluster.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Akka.Skeleton.Cluster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .WriteTo.Console())
                .ConfigureServices(services =>
                {
                    services.AddAkka("skeleton-service", builder =>
                    {
                        builder.ConfigureLoggers(configLoggers =>
                        {
                            configLoggers.LogLevel = Event.LogLevel.DebugLevel;
                            configLoggers.LogConfigOnStart = true;
                            configLoggers.ClearLoggers();
                            configLoggers.AddLogger<SerilogLogger>();
                        })
                        .WithRemoting(port: 0)
                        .WithClustering(new ClusterOptions()
                        {
                            SeedNodes = ["akka.tcp://skeleton-service@localhost:5053"],
                            Roles = ["main"]
                        })
                        .WithDistributedPubSub("main")
                        .WithActors((actorSystem, registry) =>
                        {
                            var echoActor = actorSystem.ActorOf<EchoActor>($"echo-actor");
                        });
                    });
                    services.AddHostedService<AkkaHostedService>();
                }).Build().Run();
        }
    }
}
