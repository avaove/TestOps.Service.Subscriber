using StackExchange.Redis;
using TestOps.Subscribers.Interfaces;


namespace TestOps.Subscribers
{
    public abstract class RedisSubscriber : AbstractSubscriber
    {
        protected RedisSubscriber(ISubscriptionActionHandler subscriptionActionHandler) : base(subscriptionActionHandler) { }
        public abstract Action<RedisChannel, RedisValue> GetSubscribeHandler();
    }
}
