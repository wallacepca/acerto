using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Acerto.Shared.Infrastructure.Caching
{
    public sealed class DistributedCacheManager : IDistributedCacheManager
    {
        private readonly IDistributedCache _cache;

        public DistributedCacheManager(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string name, CancellationToken cancellationToken = default)
        {
            var data = await _cache.GetAsync(name, cancellationToken);
            if (data == null || data.Length == 0)
            {
                return default;
            }

            return await ByteArrayToObjectAsync<T>(data);
        }

        public async Task SetAsync<T>(string name, T data, TimeSpan slidingExpiration, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };
            await SetAsync(name, data, options, cancellationToken);
        }

        public async Task SetAsync<T>(string name, T data, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            var arrBytes = await ObjectToByteArrayAsync(data);
            await _cache.SetAsync(name, arrBytes, options, cancellationToken);
        }

        public async Task RemoveAsync(string name, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(name, cancellationToken);
        }

        public async Task RefrehAsync(string name, CancellationToken cancellationToken = default)
        {
            await _cache.RefreshAsync(name, cancellationToken);
        }

        private static async Task<byte[]> ObjectToByteArrayAsync<T>(T data)
        {
            using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, data);

            return ms.ToArray();
        }

        private static async Task<T?> ByteArrayToObjectAsync<T>(byte[] arrBytes)
        {
            using var ms = new MemoryStream();
            await ms.WriteAsync(arrBytes, 0, arrBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);

            return await JsonSerializer.DeserializeAsync<T>(ms);
        }
    }
}
