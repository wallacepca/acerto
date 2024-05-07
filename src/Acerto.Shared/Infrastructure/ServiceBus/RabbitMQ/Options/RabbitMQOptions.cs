namespace Acerto.Shared.Infrastructure.ServiceBus.RabbitMQ.Options
{
    public sealed class RabbitMQOptions
    {
        public RabbitMQOptions()
        {
        }

        public string ConnectionString { get; set; } = string.Empty;
        public int? NumberOfWorkers { get; set; }
        public int? MaxParallelism { get; set; }
    }
}
