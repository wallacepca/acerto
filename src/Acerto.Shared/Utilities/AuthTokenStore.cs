using Microsoft.Extensions.Caching.Memory;

namespace Acerto.Products.SDK
{
    internal sealed class AuthTokenStore : IAuthTokenStore
    {
        private readonly IMemoryCache _memoryCache;

        public AuthTokenStore(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T?> GetTokenAsync<T>()
        {
            var token = _memoryCache.Get<T>(typeof(T).Name);
            return await Task.FromResult(token);
        }

        public async Task<T> SetTokenAsync<T>(T token)
        {
            _memoryCache.Set(typeof(T).Name, token);
            return await Task.FromResult(token);
        }
    }
}
