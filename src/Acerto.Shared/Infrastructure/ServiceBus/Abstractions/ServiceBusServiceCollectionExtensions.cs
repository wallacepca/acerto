using Acerto.Shared.Extensions;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, Action<ServiceBusOptions> setupOptions)
        {
            services.AddOptions();
            services.Configure(setupOptions);

            services.AddSingleton<IServiceBusSubscriber, ServiceBusSubscriber>();

            return services;
        }

        public static IServiceCollection AddMessageConsumer<TConsumer>(this IServiceCollection services)
        {
            var consumerType = typeof(TConsumer);

            var implementedHandlerInterfaces = consumerType.GetInterfacesFromTypesOf(ServiceBusMessageHandlerInterfaces.MessageHandlerInterfaceTypes);

            if (implementedHandlerInterfaces.Any())
            {
                services.RegisterType(consumerType, implementedHandlerInterfaces);
            }

            return services;
        }

        private static void RegisterType(this IServiceCollection services, Type type, IEnumerable<Type> implementedHandlerInterfaces)
        {
            foreach (var handlerInterface in implementedHandlerInterfaces)
            {
                services.AddTransient(handlerInterface, type);
            }
        }
    }
}
