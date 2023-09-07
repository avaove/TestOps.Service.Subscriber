#nullable disable warnings

using Ceridian.Framework.Core.Connector.Models;
using Ceridian.Framework.Core.Lifecycle.Mappers;
using Ceridian.Framework.Core.Lifecycle.Models;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors.Models;


namespace TestOps.Service.Subscriber.Connectors.Mappers
{
    /// <summary>
    /// Mapper for <see cref="RedisSubscriptionConnectorMapper"/>
    /// </summary>
    public class RedisSubscriptionConnectorMapper : AbstractConnectorMapper<RedisSubscription, RedisChannel>
    {
        public RedisSubscriptionConnectorMapper() {  }

        /// <summary>
        /// Prepares a request from the business layer for consumption by the connector layer. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>RestFWConnectorRequest</returns>
        public override RedisFWConnectorRequest<RedisSubscription> MapFromConnectorRequest(AbstractConnectorRequest<RedisSubscription> request) =>
            new() { Payload = request.Payload };

        /// <summary>
        /// Prepares a framework response for consumption by the business layer. 
        /// </summary>
        /// <param name="response"></param>
        /// <returns>ConnectorResponse</returns>
        public override ConnectorResponse<RedisChannel> MapToConnectorResponse(AbstractFWConnectorResponse<RedisChannel> response) =>
            new() { Payload = response.Payload };
    }
}
