using Ceridian.Framework.Core.Lifecycle.Models;
using NUnit.Framework;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Data.Shared.Framework.Options;
using TestOps.Service.Subscriber.ActionHandlers.Mappers;
using TestOps.Service.Subscriber.Connectors.Models;
using TestOps.Subscribers.Enums;
using TestOps.Subscribers.Models;


namespace TestOps.Service.Subscriber.Tests.ActionHandlers.Mappers
{
    internal class RedisSubscriptionActionHandlerMapperTests
    {
        private RedisSubscriptionActionHandlerMapper mapper;

        [SetUp]
        public void Setup() => mapper ??= new();

        [Test, Category("Redis"), Category("Subscription")]
        public void Map_Handler_Request_Creates_Payload()
        {
            var payload = new RedisSubscription() 
            {
                Channel = "BAZINGA_CHANNEL",
                ChannelHandler = (_, _) => { Thread.Sleep(0); },
                Options = new RedisConnectorConfig()
                {
                    ConnectionString = "http://pizzaz.com,password=123",
                    RetryAttempts = 1,
                    RetryWaitSeconds = 5
                }
            };

            ActionHandlerRequest<SubscriptionRequest> handlerRequest = new()
            {
                Payload = new()
                {
                    TargetFramework = SubscriptionFramework.Redis,
                    RedisSubscription = payload
                }
            };

            var connectorRequest = mapper.MapFromActionHandlerRequest(handlerRequest);

            Assert.That(connectorRequest, Has.Property("Payload").EqualTo(payload));
        }

        [Test, Category("Redis"), Category("Subscription")]
        public void Map_Handler_Response_Creates_Response()
        {
            var channel = new RedisChannel("BAZINGA_CHANNEL", RedisChannel.PatternMode.Literal);
            ConnectorResponse<RedisChannel> response = new()
            {
                Payload = channel
            };

            var handlerResponse = mapper.MapToActionHandlerResponse(response);

            Assert.Multiple(() =>
            {
                Assert.That(handlerResponse, Has.Property("ApiResponse").Not.Null);
                Assert.That(handlerResponse!.ApiResponse, Has.Property("ApiResult").EqualTo(channel));
            });
        }
    }
}
