using Ceridian.Framework.Core.Lifecycle.Models;
using TestOps.Subscribers.Models;


namespace TestOps.Subscribers.Interfaces
{
    public interface ISubscriptionActionHandler
    {
        Task<ActionHandlerResponse<object>> HandleSubscribeAsync(ActionHandlerRequest<SubscriptionRequest> request);
        Task<ActionHandlerResponse<object>> HandleUnsubscribeAsync(ActionHandlerRequest<SubscriptionRequest> request);
    }
}