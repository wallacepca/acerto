namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public interface IServiceBus
    {
        Task DeferAsync(TimeSpan delay, object message, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default);
        Task PublishAsync(object eventMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default);
        Task ReplyAsync(object replyMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default);
        Task SendAsync(object commandMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default);
    }
}
