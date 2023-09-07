#nullable disable warnings

using Ceridian.Framework.Core.Lifecycle;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Interfaces;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors.Interfaces;
using TestOps.Service.Subscriber.Connectors.Mappers;
using TestOps.Service.Subscriber.Connectors.Models;
using TestOps.Subscribers.Models;

namespace TestOps.Service.Subscriber.Connectors
{
    /// <summary>
    /// Connector to subscribe to a channel in Redis.
    /// </summary>
    public class RedisSubscriptionConnector : 
        AbstractConnector<ConnectorRequest<RedisSubscription>, ConnectorResponse<RedisChannel>, RedisSubscription, RedisChannel>, 
        IRedisSubscriptionConnector
    {
        private readonly RedisSubscriptionConnectorMapper redisConnectorMapper;
        private readonly IRedisSubscriptionFWConnector redisFWConnector;

        /// <summary>
        /// Normally expected to be instantiated via <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="redisConnectorMapper"></param>
        /// <param name="redisFWConnector"></param>
        public RedisSubscriptionConnector(RedisSubscriptionConnectorMapper redisConnectorMapper, IRedisSubscriptionFWConnector redisFWConnector)
        {
            this.redisConnectorMapper = redisConnectorMapper;
            this.redisFWConnector = redisFWConnector;
        }

        /// <summary>
        /// Not implemented, will throw <see cref="NotImplementedException"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task<ConnectorResponse<RedisChannel>> ConnectAsync(ConnectorRequest<RedisSubscription> request) =>
            throw new NotImplementedException();

        /// <summary>
        /// Calls framework connector to subscribe.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ConnectorResponse</returns>
        public async Task<ConnectorResponse<RedisChannel>> SubscribeAsync(ConnectorRequest<RedisSubscription> request)
        {
            var fwConnectorRequest = redisConnectorMapper.MapFromConnectorRequest(request);
            var fwConnectorResponse = await redisFWConnector.DoSubscribeAsync(fwConnectorRequest);

            return redisConnectorMapper.MapToConnectorResponse(fwConnectorResponse);
        }

        /// <summary>
        /// Calls framework connector to unsubscribe.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ConnectorResponse</returns>
        public async Task<ConnectorResponse<RedisChannel>> UnsubscribeAsync(ConnectorRequest<RedisSubscription> request)
        {
            var fwConnectorRequest = redisConnectorMapper.MapFromConnectorRequest(request);
            var fwConnectorResponse = await redisFWConnector.DoUnsubscribeAsync(fwConnectorRequest);

            return redisConnectorMapper.MapToConnectorResponse(fwConnectorResponse);
        }
    }
}
