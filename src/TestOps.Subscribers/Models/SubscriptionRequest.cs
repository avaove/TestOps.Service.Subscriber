#nullable disable warnings

using TestOps.Data.Shared.Framework.FWConnectors.Models;
using TestOps.Subscribers.Enums;


namespace TestOps.Subscribers.Models
{
    /// <summary>
    /// Request model containing sub/pub framework-specific request models and configuration data.
    /// Subscriber implementations are responsible for initializing only the request models and 
    /// configurations pertaining to their target framework (ignore all remaining properties).
    /// </summary>
    public class SubscriptionRequest
    {
        public SubscriptionRequest() { }
        public SubscriptionFramework TargetFramework { get; init; }
        public RedisSubscription RedisSubscription { get; init; }
    }

    /// <summary>
    /// Represents a request model specifically for subscribing to a pub/sub framework. 
    /// </summary>
    public class SubscribeRequest : SubscriptionRequest
    {
        public SubscribeRequest() : base() { }
    }

    /// <summary>
    /// Represents a request model specifically for unsubscribing to a pub/sub framework. 
    /// </summary>
    public class UnsubscribeRequest : SubscriptionRequest
    {
        public UnsubscribeRequest() : base() { }
    }
}
