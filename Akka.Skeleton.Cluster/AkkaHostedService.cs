using Akka.Actor;
using Akka.Hosting;
using Akka.Cluster;
using Akka.Skeleton.Cluster.Actors;
using Microsoft.Extensions.Hosting;
using Serilog;
using Akka.Cluster.Tools.PublishSubscribe;

namespace Akka.Skeleton.Cluster
{
    internal class AkkaHostedService : IHostedService
    {
        private ActorRegistry _registry;
        private ActorSystem _actorSystem;
        public AkkaHostedService(ActorSystem actorSystem, ActorRegistry actorRegistry)
        {
            _registry = actorRegistry;
            _actorSystem = actorSystem;
        }
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            Log.Error("Entered StopAsync");
            return _actorSystem.Terminate().WaitAsync(cancellationToken);
        }
    }
}
