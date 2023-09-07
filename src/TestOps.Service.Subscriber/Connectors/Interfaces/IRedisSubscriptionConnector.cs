using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors.Models;


namespace TestOps.Service.Subscriber.Connectors.Interfaces
{
    public interface IRedisSubscriptionConnector
    {
        Task<ConnectorResponse<RedisChannel>> SubscribeAsync(ConnectorRequest<RedisSubscription> request);
        Task<ConnectorResponse<RedisChannel>> UnsubscribeAsync(ConnectorRequest<RedisSubscription> request);
    }
}
