using Moq;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Interfaces;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors;
using TestOps.Service.Subscriber.Connectors.Mappers;
using TestOps.Service.Subscriber.Tests.TestResources;


namespace TestOps.Service.Subscriber.Tests.Connectors
{
    internal class RedisSubscriptionConnectorMockFactory : MockFactoryBase<RedisSubscriptionConnector>
    {
        public Mock<IRedisSubscriptionFWConnector>? MockRedisSubscriptionFWConnector { get; private set; }

        public override RedisSubscriptionConnector CreateMockInstance()
        {
            MockRedisSubscriptionFWConnector = CreateMockRedisSubscriptionFWConnector();

            return new(new RedisSubscriptionConnectorMapper(), MockRedisSubscriptionFWConnector.Object);
        }

        private static Mock<IRedisSubscriptionFWConnector> CreateMockRedisSubscriptionFWConnector()
        {
            Mock<IRedisSubscriptionFWConnector> mockConnector = new();

            mockConnector
                .Setup(moq => moq.DoSubscribeAsync(It.IsAny<RedisFWConnectorRequest<RedisSubscription>>()))
                .ReturnsAsync(new RedisFWConnectorResponse<RedisChannel>()
                {
                    Payload = new RedisChannel("BAZINGA_CHANNEL", RedisChannel.PatternMode.Literal)
                }).Verifiable();
            mockConnector
                .Setup(moq => moq.DoUnsubscribeAsync(It.IsAny<RedisFWConnectorRequest<RedisSubscription>>()))
                .ReturnsAsync(new RedisFWConnectorResponse<RedisChannel>()
                {
                    Payload = new RedisChannel("BAZINGA_CHANNEL", RedisChannel.PatternMode.Literal)
                }).Verifiable();

            return mockConnector;
        }
    }
}
