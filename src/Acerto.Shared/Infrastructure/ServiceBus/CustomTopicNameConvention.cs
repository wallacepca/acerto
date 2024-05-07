using Rebus.Topic;

namespace Acerto.Shared.Infrastructure.ServiceBus
{
    internal sealed class CustomTopicNameConvention : ITopicNameConvention
    {
        public string GetTopic(Type eventType)
        {
            return eventType.FullName?.ToLowerInvariant() ?? eventType.Name;
        }
    }
}
