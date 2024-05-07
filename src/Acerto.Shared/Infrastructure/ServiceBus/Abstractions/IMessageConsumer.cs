namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public interface IMessageConsumer<in TMessage> : IMessageConsumer, Rebus.Handlers.IHandleMessages<TMessage>
    {
    }

    public interface IMessageConsumer : Rebus.Handlers.IHandleMessages
    {
    }
}
