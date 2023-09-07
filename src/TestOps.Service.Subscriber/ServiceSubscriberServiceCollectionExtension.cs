using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestOps.Data.Shared;
using TestOps.Data.Shared.Framework.Options;
using TestOps.Service.Subscriber.ActionHandlers;
using TestOps.Service.Subscriber.Connectors;
using TestOps.Service.Subscriber.Connectors.Interfaces;
using TestOps.Subscribers.Interfaces;
using TestOps.Subscribers.VideoRecording.Options;


namespace TestOps.Service.Subscriber
{
    public static class ServiceSubscriberServiceCollectionExtension
    {
        public static void AddSubscriptionFrameworks(this IServiceCollection services)
        {
            services.AddScoped<ISubscriptionActionHandler, SubscriptionActionHandler>();
            services.AddScoped<IRedisSubscriptionConnector, RedisSubscriptionConnector>();
            services.AddRedis();

            services.AddSubscriberServiceConfigs();
        }

        private static void AddSubscriberServiceConfigs(this IServiceCollection services)
        {
            services.AddScoped(provider =>
                provider.GetService<IConfiguration>()!.GetSection(RedisConnectorConfig.CONFIG_SECTION_NAME).Get<RedisConnectorConfig>());
            services.AddScoped(provider =>
                provider.GetService<IConfiguration>()!.GetSection(GridApiConfig.CONFIG_SECTION_NAME).Get<GridApiConfig>());
        }
    }
}
