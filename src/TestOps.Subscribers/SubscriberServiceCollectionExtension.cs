using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestOps.Data.Shared.KubernetesClient;
using TestOps.Data.Shared.Redis;
using TestOps.Subscribers.VideoRecording;
using TestOps.Subscribers.VideoRecording.Options;


namespace TestOps.Subscribers
{
    public static class SubscriberServiceCollectionExtension
    {
        public static IConfigurationBuilder AddSubscriberAppSettings(this IConfigurationBuilder configBuilder)
        {
            configBuilder.AddJsonFile("appsettings.video.json", optional: true, reloadOnChange: true);

            return configBuilder;
        }

        public static void AddSubscribers(this IServiceCollection services)
        {
            services.AddSubscriberConfigs();

            services.AddHostedService<VideoRecordingSubscriber>();
        }

        private static void AddSubscriberConfigs(this IServiceCollection services)
        {
            services.AddScoped(provider =>
                provider.GetService<IConfiguration>()!.GetSection(VideoJobConfig.CONFIG_SECTION_NAME).Get<VideoJobConfig>());
        }
    }
}
