using Akka.Actor;
using Akka.Event;

namespace Akka.Skeleton.Actors
{
    internal class EchoActor : ReceiveActor
    {
        public EchoActor()
        {
            Receive<string>(msg =>
            {
                Context.GetLogger().Info($"EchoActor: Received {msg}");
                Sender.Tell(msg);
            });
            Receive<int>(msg =>
            {
                Context.GetLogger().Info($"EchoActor: Received {msg}");
                Sender.Tell(msg);
            });
            Receive<object>(msg =>
            {
                Context.GetLogger().Error($"EchoActor: Received Unsupported Type {msg.GetType().Name}");
            });
        }
    }
}
