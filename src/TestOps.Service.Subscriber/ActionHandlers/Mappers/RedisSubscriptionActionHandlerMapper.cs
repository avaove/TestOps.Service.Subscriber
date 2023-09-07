using Ceridian.Framework.Core.Lifecycle.Mappers;
using Ceridian.Framework.Core.Lifecycle.Models;
using StackExchange.Redis;
using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Service.Subscriber.Connectors.Models;
using TestOps.Subscribers.Models;

namespace TestOps.Service.Subscriber.ActionHandlers.Mappers
{
    /// <summary>
    /// Mapper for <see cref="RedisSubscriptionActionHandlerMapper"/>
    /// </summary>
    public class RedisSubscriptionActionHandlerMapper : AbstractActionHandlerMapper<SubscriptionRequest, RedisSubscription, RedisChannel, object>
    {
        /// <summary>
        /// Maps from a <see cref="ActionHandlerRequest{T}"/> to a <see cref="ConnectorRequest{T}"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns>ConnectorRequest</returns>
        public override ConnectorRequest<RedisSubscription> MapFromActionHandlerRequest(AbstractActionHandlerRequest<SubscriptionRequest> request) => 
            new() { Payload = request.Payload?.RedisSubscription };

        /// <summary>
        /// Not implemented, Will throw <see cref="NotSupportedException"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override AbstractConnectorRequest<RedisSubscription> MapFromActionHandlerRequest(AbstractActionHandlerRequest<SubscriptionRequest> request, 
            List<AbstractActionHandlerResponse<object>> responses) =>
            throw new NotSupportedException();

        /// <summary>
        /// Maps <see cref="ConnectorResponse{T}" /> to a <see cref="ActionHandlerResponse{T}" />
        /// </summary>
        /// <param name="response"></param>
        /// <returns>ActionHandlerResponse</returns>
        public override ActionHandlerResponse<object> MapToActionHandlerResponse(AbstractConnectorResponse<RedisChannel> response) =>
            new()
            {
                Headers = response.Headers,
                ApiResponse = new() { ApiResult = response.Payload }
            };
    }
}
