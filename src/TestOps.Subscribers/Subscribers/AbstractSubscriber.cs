using Microsoft.Extensions.Hosting;
using TestOps.Subscribers.Interfaces;


namespace TestOps.Subscribers
{
    public abstract class AbstractSubscriber : IHostedService
    {
        protected CancellationTokenSource cancellationTokenSource;
        protected readonly ISubscriptionActionHandler subscriptionActionHandler;
        protected AbstractSubscriber(ISubscriptionActionHandler subscriptionActionHandler)
        {
            this.subscriptionActionHandler = subscriptionActionHandler;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}