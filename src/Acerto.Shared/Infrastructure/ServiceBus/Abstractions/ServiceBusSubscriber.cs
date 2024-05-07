using Rebus.Bus;

namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public sealed class ServiceBusSubscriber : IServiceBusSubscriber
    {
        private readonly IBus _bus;

        public ServiceBusSubscriber(IBus bus)
        {
            _bus = bus;
        }

        public Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default) => _bus.Subscribe<TEvent>();
        public Task SubscribeAsync(Type eventType, CancellationToken cancellationToken = default) => _bus.Subscribe(eventType);
        public Task UnsubscribeAsync<TEvent>(CancellationToken cancellationToken = default) => _bus.Unsubscribe<TEvent>();
        public Task UnsubscribeAsync(Type eventType, CancellationToken cancellationToken = default) => _bus.Unsubscribe(eventType);
    }
}
