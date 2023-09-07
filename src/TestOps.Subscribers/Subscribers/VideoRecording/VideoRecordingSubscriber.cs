#pragma warning disable CA2254 // Template should be a static expression

using System.Text;
using Ceridian.Framework.Core.Lifecycle.Models;
using k8s;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Data.Shared.Framework.Options;
using TestOps.Data.Shared.KubernetesClient;
using TestOps.Subscribers.Enums;
using TestOps.Subscribers.Models;
using TestOps.Subscribers.Interfaces;
using TestOps.Subscribers.VideoRecording.Models;
using TestOps.Subscribers.VideoRecording.Options;


namespace TestOps.Subscribers.VideoRecording
{
    public class VideoRecordingSubscriber : RedisSubscriber, IDisposable
    {
        private const string ChannelName = "VIDEO_RECORDING_CHANNEL";
        private const int MaxRetries = 3;
        private const int RetryDelayStartSeconds = 1;

        private readonly IKubernetesClientProvider kubernetesProvider;
        private readonly VideoJobConfig jobConfig;
        private readonly GridApiConfig gridConfig;
        private readonly RedisConnectorConfig redisConfig;
        private readonly ILogger logger;
        private bool disposedValue;

        public VideoRecordingSubscriber(ISubscriptionActionHandler subscriptionActionHandler,
            IKubernetesClientProvider kubernetesProvider,
            RedisConnectorConfig redisConfig,
            VideoJobConfig jobConfig,
            GridApiConfig gridConfig,
            ILogger<VideoRecordingSubscriber> logger) : base(subscriptionActionHandler)
        {
            this.kubernetesProvider = kubernetesProvider;
            this.jobConfig = jobConfig;
            this.gridConfig = gridConfig;
            this.redisConfig = redisConfig;
            this.logger = logger;
        }

        ~VideoRecordingSubscriber()
        {
            Dispose(disposing: false);
        }

        public override async Task StartAsync(CancellationToken cancellationToken) =>
            await SubscribeWithRetry(cancellationToken);

        private TRequest CreateSubscriptionRequest<TRequest>() where TRequest : SubscriptionRequest, new() =>
            new()
            {
                TargetFramework = SubscriptionFramework.Redis,
                RedisSubscription = new RedisSubscription
                {
                    Channel = ChannelName,
                    ChannelHandler = GetSubscribeHandler(),
                    Options = redisConfig
                }
            };
        
        private async Task SubscribeWithRetry(CancellationToken cancellationToken)
        {
            bool isSubscribed = false;
            int retryDelaySeconds = RetryDelayStartSeconds;
            var subscriptionRequestPayload = CreateSubscriptionRequest<SubscribeRequest>();

            for (int retryCount = 0; retryCount < MaxRetries && !isSubscribed; retryCount++)
            {
                try
                {
                    await subscriptionActionHandler.HandleSubscribeAsync(
                        new ActionHandlerRequest<SubscriptionRequest>() { Payload = subscriptionRequestPayload }
                    );

                    isSubscribed = true;
                    logger.LogInformation($"Successfully subscribed to {ChannelName}.");
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Could not subscribe to {ChannelName}. Reason: {ex.Message}. Retry Attempt: {retryCount + 1}...");

                    retryDelaySeconds += retryDelaySeconds;
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), cancellationToken);
                }
            }

            if (!isSubscribed)
            {
                try
                {
                    await subscriptionActionHandler.HandleSubscribeAsync(
                        new ActionHandlerRequest<SubscriptionRequest>() { Payload = subscriptionRequestPayload }
                    );

                    isSubscribed = true;
                    logger.LogInformation($"Successfully subscribed to {ChannelName}.");
                }
                catch
                {
                    logger.LogInformation($"Subscription failed after {MaxRetries} attempts.");

                    await StopAsync(cancellationToken);
                }
            }
        }

        public override Action<RedisChannel, RedisValue> GetSubscribeHandler() => async (channel, sessionId) =>
        {
            try
            {
                logger.LogInformation($"Received session {sessionId} on channel {channel}.");

                var ipAddress = await GetNodeIpAddressAsync();
                if (string.IsNullOrEmpty(ipAddress))
                    throw new ArgumentNullException(ipAddress, "Current session isn't active on any node.");

                logger.LogInformation($"Session is running on Node with IP address {ipAddress}.");

                StartVideoRecordingJob();

                logger.LogInformation($"Session {sessionId} recording has started.");


                async Task<string?> GetNodeIpAddressAsync()
                {
                    string query = $@"
                        query {{
                            session(id: ""{sessionId}"") {{
                            nodeUri
                            }}
                        }}
                    ";

                    using HttpClient httpClient = new();
                    var request = new HttpRequestMessage(HttpMethod.Post, gridConfig.GridGraphQlEndpoint)
                    {
                        Content = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(new { query }),
                            Encoding.UTF8,
                            "application/json"
                            )
                    };

                    var response = await httpClient!.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var nodeUri = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseData>(responseContent)?.Data?.Session?.NodeUri;

                    if (string.IsNullOrEmpty(nodeUri))
                        return null;

                    var ipAddress = new Uri(nodeUri!).Host;
                    if (string.IsNullOrEmpty(ipAddress))
                        return null;

                    return ipAddress;
                }

                void StartVideoRecordingJob()
                {
                    using Kubernetes kubernetesClient = kubernetesProvider.Get();
                    var jobEnvironmentVariables = jobConfig.JobEnvironmentVariables;
                    var videoJob = new VideoJobBuilder(jobConfig)
                        .WithJobNameIdentifier(sessionId!)
                        .WithEnvironmentVariable(jobEnvironmentVariables["SessionId"].Name, sessionId!)
                        .WithEnvironmentVariable(jobEnvironmentVariables["IpAddress"].Name, ipAddress!)
                        .WithEnvironmentVariable(jobEnvironmentVariables["AzureAccountName"].Name, jobEnvironmentVariables["AzureAccountName"].Value)
                        .WithEnvironmentVariable(jobEnvironmentVariables["AzureBlobName"].Name, jobEnvironmentVariables["AzureBlobName"].Value)
                        .WithEnvironmentVariable(jobEnvironmentVariables["AzureBlobStorageKey"].Name, jobEnvironmentVariables["AzureBlobStorageKey"].Value)
                        .Build();

                    kubernetesClient.CreateNamespacedJob(videoJob, jobConfig.Namespace);
                }

            } 
            catch (Exception ex)
            {
                logger.LogError($"Recording for session {sessionId} will not take place. {ex.Source}: {ex.Message}");
            }
        };

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Terminating video recording subscriber service...");

            try
            {
                await subscriptionActionHandler.HandleUnsubscribeAsync(
                    new ActionHandlerRequest<SubscriptionRequest>() { Payload = CreateSubscriptionRequest<UnsubscribeRequest>() }
                );
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Could not unsubscribe from {ChannelName}. Reason: {ex.Message}.");
            }
            finally
            {
                Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationTokenSource?.Cancel();
                    cancellationTokenSource?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
