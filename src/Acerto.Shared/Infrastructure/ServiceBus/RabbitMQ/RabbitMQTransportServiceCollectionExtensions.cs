using System.Reflection;
using Acerto.Shared.Infrastructure.ServiceBus;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions.Options;
using Acerto.Shared.Infrastructure.ServiceBus.RabbitMQ.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Topic;
using Rebus.Transport.InMem;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQTransportServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQTransport(this IServiceCollection services, Action<RabbitMQOptions> setupOptions)
        {
            services.AddOptions();
            services.Configure(setupOptions);

            services.AddRebus((config, serviceProvider) =>
            {
                // o nome da fila será o nome do assembly automaticamente
                var inputQueueName = $"{Assembly.GetEntryAssembly()!.GetName().Name!.ToLowerInvariant()}";

                var serviceBusOptions = serviceProvider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                if (serviceBusOptions.Enabled)
                {
                    config.Transport(c =>
                        c.UseRabbitMq(
                            serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value.ConnectionString,
                            inputQueueName).ExchangeNames("Acerto.direct", "Acerto.topics"));
                }
                else
                {
                    config.Transport(c => c.UseInMemoryTransport(new InMemNetwork(false), inputQueueName));
                }

                config.Logging(options => options.MicrosoftExtensionsLogging(LoggerFactory.Create(options => options.AddConsole())));

                config.Options(options =>
                {
                    var rabbitMQOptions = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                    // Isso é para customizar o nome das routing keys usando o full name do tipo.
                    options.UseCustomTopicNameConvention();
                    options.SetNumberOfWorkers(rabbitMQOptions?.NumberOfWorkers ?? 1);
                    options.SetMaxParallelism(rabbitMQOptions?.MaxParallelism ?? 5);
                });

                config.Routing(c => c.TypeBased());

                return config;
            });

            services.AddSingleton<IServiceBus, RebusWrapper>();

            return services;
        }

        private static OptionsConfigurer UseCustomTopicNameConvention(this OptionsConfigurer options)
        {
            options.Register<ITopicNameConvention>(context => new CustomTopicNameConvention());
            return options;
        }
    }
}
