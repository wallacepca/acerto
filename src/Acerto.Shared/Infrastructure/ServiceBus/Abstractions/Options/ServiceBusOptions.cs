namespace Acerto.Shared.Infrastructure.ServiceBus.Abstractions.Options
{
    public sealed class ServiceBusOptions
    {
        public bool Enabled { get; set; } = true;
        public bool AutoRegisterConsumers { get; set; } = true;
    }
}
