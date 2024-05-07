using Acerto.Shared.Extensions;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Rebus.Bus;

namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions
{
    public static class ServiceBusServiceProviderExtensions
    {
        public static IHost UseServiceBus(this IHost host)
        {
            var serviceBusOptions = host.Services.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

            if (serviceBusOptions.Enabled)
            {
                if (serviceBusOptions.AutoRegisterConsumers)
                {
                    var bus = host.Services.GetRequiredService<IBus>();
                    bus.SubscribeMessageConsumers();
                }
            }

            return host;
        }

        private static void SubscribeMessageConsumers(this IBus bus)
        {
            var messageTypes = TypeExtensions.InternalLoadedAssemblies.SelectMany(a => a.GetTypes())
                .Where(TypeExtensions.IsConcrete)
                .Select(type => type.GetInterfacesFromTypesOf(ServiceBusMessageHandlerInterfaces.MessageHandlerInterfaceTypes))
                .Where(interfaces => interfaces.Any())
                .SelectMany(interfaces => interfaces.SelectMany(type => type.GetGenericArguments()));

            foreach (var messageType in messageTypes)
            {
                _ = bus.Subscribe(messageType);
            }
        }
    }
}
