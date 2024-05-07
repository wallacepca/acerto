namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public sealed class ServiceBusMessageHandlerInterfaces
    {
        public static readonly Type[] MessageHandlerInterfaceTypes =
            [
                typeof(Rebus.Handlers.IHandleMessages<>)
            ];
    }
}
