using Microsoft.Extensions.Caching.Distributed;

namespace Acerto.Shared.Infrastructure.Caching
{
    public interface IDistributedCacheManager
    {
        Task<T?> GetAsync<T>(string name, CancellationToken cancellationToken = default);
        Task RefrehAsync(string name, CancellationToken cancellationToken = default);
        Task RemoveAsync(string name, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string name, T instance, TimeSpan slidingExpiration, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string name, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);
    }
}
