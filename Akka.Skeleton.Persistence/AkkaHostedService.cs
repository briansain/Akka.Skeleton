using Akka.Actor;
using Akka.Hosting;
using Akka.Skeleton.Persistence.Actors;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Akka.Skeleton.Persistence
{
    internal class AkkaHostedService : IHostedService
    {
        private ActorRegistry _registry;
        public AkkaHostedService(ActorRegistry actorRegistry)
        {
            _registry = actorRegistry;
        }
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var echoActor = _registry.Get<EchoActor>();
            var statefulActor = _registry.Get<StatefulActor>();
            while(true)
            {
                echoActor.Ask("Hello World");
                echoActor.Ask(42);
                echoActor.Ask(42.50);

                statefulActor.Tell($"HelloWorld -> {DateTime.Now}");
                Thread.Sleep(3000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            Log.Error("Entered StopAsync");
            return Task.CompletedTask;
        }
    }
}
