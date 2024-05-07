using Acerto.Shared.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services)
        {
            services.TryAddSingleton<IDistributedCacheManager, DistributedCacheManager>();

            return services;
        }
    }
}
