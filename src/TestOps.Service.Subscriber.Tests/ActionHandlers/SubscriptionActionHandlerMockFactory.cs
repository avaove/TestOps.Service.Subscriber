#nullable disable warnings

using Ceridian.Framework.Core.ErrorHandling.Models;
using Moq;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.ActionHandlers;
using TestOps.Service.Subscriber.ActionHandlers.Mappers;
using TestOps.Service.Subscriber.Connectors.Interfaces;
using TestOps.Service.Subscriber.Connectors.Models;
using TestOps.Service.Subscriber.Tests.Fakes;
using TestOps.Service.Subscriber.Tests.TestResources;


namespace TestOps.Service.Subscriber.Tests.ActionHandlers
{
    internal class SubscriptionActionHandlerMockFactory : MockFactoryBase<SubscriptionActionHandler>
    {
        public Mock<IRedisSubscriptionConnector> MockRedisSubscriptionConnector { get; private set; }
        private readonly IApiStatusFactory StatusFactory = new ApiStatusFactoryFake();

        public override SubscriptionActionHandler CreateMockInstance()
        {
            MockRedisSubscriptionConnector = CreateMockRedisSubscriptionConnector();

            return new(MockRedisSubscriptionConnector.Object, new RedisSubscriptionActionHandlerMapper(), StatusFactory);
        }

        private static Mock<IRedisSubscriptionConnector> CreateMockRedisSubscriptionConnector()
        {
            Mock<IRedisSubscriptionConnector> mockConnector = new();

            mockConnector
                .Setup(moq => moq.SubscribeAsync(It.IsAny<ConnectorRequest<RedisSubscription>>()))
                .ReturnsAsync(new ConnectorResponse<RedisChannel>()
                {
                    Payload = new RedisChannel() 
                })
                .Verifiable();
            mockConnector
                .Setup(moq => moq.UnsubscribeAsync(It.IsAny<ConnectorRequest<RedisSubscription>>()))
                .ReturnsAsync(new ConnectorResponse<RedisChannel>()
                {
                    Payload = new RedisChannel()
                })
                .Verifiable();

            return mockConnector;
        }
    }
}
