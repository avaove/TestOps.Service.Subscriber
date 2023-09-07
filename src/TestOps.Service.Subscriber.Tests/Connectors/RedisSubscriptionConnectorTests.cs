using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors;
using TestOps.Service.Subscriber.Connectors.Models;


namespace TestOps.Service.Subscriber.Tests.Connectors
{
    [TestFixture]
    internal class RedisSubscriptionConnectorTests
    {
        private RedisSubscriptionConnectorMockFactory factory;
        private RedisSubscriptionConnector connector;

        [SetUp]
        public void Setup()
        {
            factory ??= new();
            connector ??= factory.CreateMockInstance();
        }

        [Test, Category("Redis"), Category("Subscription")]
        public async Task Redis_Subscribe_Framework_Connector()
        {
            RedisSubscription redisSubscribe = new() 
            {
                Channel = new RedisChannel("BAZINGA_CHANNEL", RedisChannel.PatternMode.Literal),
                ChannelHandler = (_, _) => { Thread.Sleep(0); }
            };

            var response = await connector.SubscribeAsync(new ConnectorRequest<RedisSubscription>() { Payload = redisSubscribe });

            Assert.Multiple(() =>
            {
                Assert.That(() => VerifySubscribeAsyncCalledOnce(), Throws.Nothing);
                Assert.That(response, Has.Property("Payload").EqualTo(redisSubscribe.Channel));
            });
        }

        [Test, Category("Redis"), Category("Subscription")]
        public async Task Redis_Unsubscribe_Framework_Connector()
        {
            RedisSubscription redisSubscribe = new()
            {
                Channel = new RedisChannel("BAZINGA_CHANNEL", RedisChannel.PatternMode.Literal),
                ChannelHandler = (_, _) => { Thread.Sleep(0); }
            };

            var response = await connector.UnsubscribeAsync(new ConnectorRequest<RedisSubscription>() { Payload = redisSubscribe });

            Assert.Multiple(() =>
            {
                Assert.That(() => VerifyUnsubscribeAsyncCalledOnce(), Throws.Nothing);
                Assert.That(response, Has.Property("Payload").EqualTo(redisSubscribe.Channel));
            });
        }

        private void VerifySubscribeAsyncCalledOnce() => factory.MockRedisSubscriptionFWConnector
            !.Verify(moq => moq.DoSubscribeAsync(It.IsAny<RedisFWConnectorRequest<RedisSubscription>>()), Times.Once);

        private void VerifyUnsubscribeAsyncCalledOnce() => factory.MockRedisSubscriptionFWConnector
            !.Verify(moq => moq.DoUnsubscribeAsync(It.IsAny<RedisFWConnectorRequest<RedisSubscription>>()), Times.Once);
    }
}
