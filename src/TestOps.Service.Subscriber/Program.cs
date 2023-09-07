#nullable disable warnings

using Ceridian.Framework.Core.Infrastructure;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using TestOps.Subscribers;
using TestOps.Data.Shared;

namespace TestOps.Service.Subscriber
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; }

        private static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddSubscriberAppSettings()
                .AddEnvironmentVariables()
                .Build();

            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                        loggingBuilder.AddNLog(Configuration);
                        loggingBuilder.AddConsole();
                    });

                    services.AddSingleton(provider =>
                    {
                        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                        return loggerFactory.CreateLogger("subscribe-logger");
                    });

                    services.AddSingleton(Configuration);

                    services.AddCeridianFrameworkCore(Assembly.GetEntryAssembly());
                    services.AddSubscriptionFrameworks();
                    services.AddKubernetes();
                    services.AddSubscribers();
                })
                .Build();

            host.Run();
        }
    }
}

