using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using Serilog.Debugging;

namespace Akka.Skeleton.Cluster.Actors
{
    internal class EchoActor : ReceiveActor
    {
        public EchoActor()
        {
            Receive<string>(msg =>
            {
                Context.GetLogger().Info($"EchoActor {Self.Path.ToStringWithoutAddress()}: Received {msg} from {Sender.Path.ToStringWithAddress()}");
                //if (!msg.StartsWith("Hello Earth"))
                //{
                //    Sender.Tell(msg);
                //}
            });
            Receive<int>(msg =>
            {
                Context.GetLogger().Info($"EchoActor: Received {msg}");
                DistributedPubSub.Get(Context.System).Mediator.Tell(new SendToAll("/user/echo-actor", "hello world"));
            });
            Receive<SubscribeAck>(msg =>
            {
                Context.GetLogger().Info($"Received SubscribeAck");
            });
            Receive<object>(msg =>
            {
                Context.GetLogger().Error($"EchoActor: Received Unsupported Type {msg.GetType().Name}");
            });
            var mediator = DistributedPubSub.Get(Context.System).Mediator;
            mediator.Tell(new Put(Self));
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3), Self, 0, Self);
        }
    }
}
