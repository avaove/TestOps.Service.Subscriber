using Ceridian.Framework.Core.Lifecycle.Models;
using Moq;
using NUnit.Framework;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.ActionHandlers;
using TestOps.Service.Subscriber.Connectors.Models;
using TestOps.Subscribers.Enums;
using TestOps.Subscribers.Models;


namespace TestOps.Service.Subscriber.Tests.ActionHandlers
{
    [TestFixture]
    internal class SubscriptionActionHandlerTests
    {
        private SubscriptionActionHandlerMockFactory factory;
        private SubscriptionActionHandler handler;

        [SetUp]
        public void Setup()
        {
            factory ??= new();
            handler ??= factory.CreateMockInstance();
        }

        [TearDown]
        public void Teardown()
        {
            factory.MockRedisSubscriptionConnector.Reset();
        }

        [Test, Category("Redis"), Category("Subscription")]
        public void Subscribe_Handler_Calls_Connector()
        {
            var request = new ActionHandlerRequest<SubscriptionRequest>()
            {
                Payload = new SubscribeRequest()
                {
                    TargetFramework = SubscriptionFramework.Redis,
                    RedisSubscription = new RedisSubscription()
                }
            };

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    var response = await handler.HandleSubscribeAsync(request);
                    Assert.That(response, Is.Not.Null.And.InstanceOf<ActionHandlerResponse<object>>());
                });
                Assert.That(() => VerifySubscribeAsyncCalledTimes(Times.Once), Throws.Nothing);
                Assert.That(() => VerifyUnsubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
            }); 
        }

        [Test, Category("Redis"), Category("Subscription")]
        public void Subscribe_Handler_Throws_Exception()
        {
            var request = new ActionHandlerRequest<SubscriptionRequest>()
            {
                Payload = new SubscribeRequest()
                {
                    TargetFramework = (SubscriptionFramework)1, // Invalid TargetFramework.
                    RedisSubscription = new RedisSubscription()
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(() => handler.HandleSubscribeAsync(request), 
                    Throws.Exception.With.Message.EqualTo($"An error occurred while processing {request?.Payload.GetType()}"));
                Assert.That(() => VerifySubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
                Assert.That(() => VerifyUnsubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
            });
        }

        [Test, Category("Redis"), Category("Subscription")]
        public void Unsubscribe_Handler_Calls_Connector()
        {
            var request = new ActionHandlerRequest<SubscriptionRequest>()
            {
                Payload = new UnsubscribeRequest()
                {
                    TargetFramework = SubscriptionFramework.Redis,
                    RedisSubscription = new RedisSubscription()
                }
            };

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    var response = await handler.HandleUnsubscribeAsync(request);
                    Assert.That(response, Is.Not.Null.And.InstanceOf<ActionHandlerResponse<object>>());
                });
                Assert.That(() => VerifyUnsubscribeAsyncCalledTimes(Times.Once), Throws.Nothing);
                Assert.That(() => VerifySubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
            });
        }

        [Test, Category("Redis"), Category("Subscription")]
        public void Unsubscribe_Handler_Throws_Exception()
        {
            var request = new ActionHandlerRequest<SubscriptionRequest>()
            {
                Payload = new UnsubscribeRequest()
                {
                    TargetFramework = (SubscriptionFramework)1, // Invalid TargetFramework.
                    RedisSubscription = new RedisSubscription()
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(() => handler.HandleUnsubscribeAsync(request), 
                    Throws.Exception.With.Message.EqualTo($"An error occurred while processing {request?.Payload.GetType()}"));
                Assert.That(() => VerifyUnsubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
                Assert.That(() => VerifySubscribeAsyncCalledTimes(Times.Never), Throws.Nothing);
            });
        }

        private void VerifySubscribeAsyncCalledTimes(Func<Times> times) => factory.MockRedisSubscriptionConnector
            !.Verify(moq => moq.SubscribeAsync(It.IsAny<ConnectorRequest<RedisSubscription>>()), times);

        private void VerifyUnsubscribeAsyncCalledTimes(Func<Times> times) => factory.MockRedisSubscriptionConnector
            !.Verify(moq => moq.UnsubscribeAsync(It.IsAny<ConnectorRequest<RedisSubscription>>()), times);
    }
}