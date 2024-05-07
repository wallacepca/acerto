namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public interface IServiceBusSubscriber
    {
        Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default);
        Task SubscribeAsync(Type eventType, CancellationToken cancellationToken = default);
        Task UnsubscribeAsync<TEvent>(CancellationToken cancellationToken = default);
        Task UnsubscribeAsync(Type eventType, CancellationToken cancellationToken = default);
    }
}
