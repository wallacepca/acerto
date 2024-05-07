using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;
using Rebus.Bus;

namespace Acerto.Shared.Infrastructure.ServiceBus
{
    internal sealed class RebusWrapper : IServiceBus
    {
        private readonly IBus _bus;

        public RebusWrapper(IBus bus)
        {
            _bus = bus;
        }

        public Task DeferAsync(TimeSpan delay, object message, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _bus.Defer(delay, message, optionalHeaders);
        }

        public Task PublishAsync(object eventMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _bus.Publish(eventMessage, optionalHeaders);
        }

        public Task ReplyAsync(object replyMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _bus.Reply(replyMessage, optionalHeaders);
        }

        public Task SendAsync(object commandMessage, IDictionary<string, string>? optionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _bus.Send(commandMessage, optionalHeaders);
        }
    }
}
