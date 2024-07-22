﻿using Akka.Actor;
using Akka.Hosting;
using Akka.Logger.Serilog;
using Akka.Persistence.Sql.Hosting;
using Akka.Skeleton.Persistence.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using LinqToDB;

namespace Akka.Skeleton.Persistence
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
                        .WithSqlPersistence("Host=localhost;Port=5432;database=akkaskeleton;username=postgres;password=postgrespassword;", ProviderName.PostgreSQL15)
                        //.WithSqlPersistence("Data Source=localhost;Initial Catalog=akkaskeleton;User Id=sa;Password=sqlserverPassword123;TrustServerCertificate=true", ProviderName.SqlServer)
                        .WithActors((actorSystem, registry) =>
                        {
                            var echoActor = actorSystem.ActorOf(Props.Create<EchoActor>(), "echo-actor");
                            registry.Register<EchoActor>(echoActor);

                            var statefulActor = actorSystem.ActorOf(Props.Create<StatefulActor>(), "stateful-actor");
                            registry.Register<StatefulActor>(statefulActor);
                        });
                    });
                    services.AddHostedService<AkkaHostedService>();
                }).Build().Run();
        }
    }
}
