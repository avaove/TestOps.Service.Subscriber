#nullable disable warnings

using Ceridian.Framework.Core.ErrorHandling.Enums;
using Ceridian.Framework.Core.ErrorHandling.Exceptions;
using Ceridian.Framework.Core.ErrorHandling.Models;
using Ceridian.Framework.Core.Lifecycle;
using Ceridian.Framework.Core.Lifecycle.Models;
using TestOps.Service.Subscriber.ActionHandlers.Mappers;
using TestOps.Service.Subscriber.Connectors.Interfaces;
using TestOps.Service.Subscriber.Framework;
using TestOps.Subscribers.Enums;
using TestOps.Subscribers.Interfaces;
using TestOps.Subscribers.Models;


namespace TestOps.Service.Subscriber.ActionHandlers
{
    /// <summary>
    /// Action Handler subscription logic.
    /// </summary>
    public class SubscriptionActionHandler : 
        AbstractActionHandler<ActionHandlerRequest<SubscriptionRequest>, ActionHandlerResponse<object>, SubscriptionRequest, object>, 
        ISubscriptionActionHandler
    {
        private readonly IRedisSubscriptionConnector redisConnector;
        private readonly RedisSubscriptionActionHandlerMapper redisActionHandlerMapper;
        private readonly IApiStatusFactory apiStatusFactory;

        /// <summary>
        /// ActionHandler constructor.
        /// </summary>
        /// <param name="redisConnector"></param>
        /// <param name="redisActionHandlerMapper"></param>
        /// <param name="apiStatusFactory"></param>
        public SubscriptionActionHandler(IRedisSubscriptionConnector redisConnector,
            RedisSubscriptionActionHandlerMapper redisActionHandlerMapper,
            IApiStatusFactory apiStatusFactory)
        {
            this.redisConnector = redisConnector;
            this.redisActionHandlerMapper = redisActionHandlerMapper;
            this.apiStatusFactory = apiStatusFactory;
        }

        /// <summary>
        /// Not implemented, Will throw <see cref="System.NotImplementedException"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task<ActionHandlerResponse<object>> HandleRequestAsync(ActionHandlerRequest<SubscriptionRequest> request) =>
            throw new System.NotImplementedException();

        /// <summary>
        /// Handles request to subscribe.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionHandlerResponse<object>> HandleSubscribeAsync(ActionHandlerRequest<SubscriptionRequest> request) =>
            await HandleSubscriptionAsync(request);

        /// <summary>
        /// Handles request to unsubscribe.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ActionHandlerResponse<object>> HandleUnsubscribeAsync(ActionHandlerRequest<SubscriptionRequest> request) =>
            await HandleSubscriptionAsync(request);

        private async Task<ActionHandlerResponse<object>> HandleSubscriptionAsync(ActionHandlerRequest<SubscriptionRequest> request)
        {
            if (request?.Payload?.TargetFramework is SubscriptionFramework.Redis)
            {
                var connectorRequest = redisActionHandlerMapper.MapFromActionHandlerRequest(request);

                var connectorResponse = request.Payload is SubscribeRequest
                    ? await redisConnector.SubscribeAsync(connectorRequest)
                    : await redisConnector.UnsubscribeAsync(connectorRequest);

                return redisActionHandlerMapper.MapToActionHandlerResponse(connectorResponse);
            }
            else
            {
                string errMsg = $"An error occurred while processing {request?.Payload.GetType()}";
                var status = apiStatusFactory.Create(SubscriberStatusCode.InvalidRequest, severity: Severity.ERROR);

                throw new BadRequestException(status, errMsg);
            }
        }
    }
}
