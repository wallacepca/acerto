using Acerto.Auth.SDK;
using Acerto.Products.SDK;
using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Caching;
using Acerto.Shared.Infrastructure.Caching;
using Microsoft.Extensions.Options;
using NetDevPack.Identity.Jwt.Model;
using NetDevPack.Identity.Model;

namespace Acerto.Orders.Service.Services
{
    public sealed class ProductInfoProvider : IProductInfoProvider
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IAuthTokenStore _authTokenStore;
        private readonly IOptionsMonitor<AuthApiClientOptions> _authOptions;
        private readonly IProductsApiClient _productsApiClient;
        private readonly IDistributedCacheManager _distributedCacheManager;

        public ProductInfoProvider(
            IAuthApiClient authApiClient,
            IAuthTokenStore authTokenStore,
            IOptionsMonitor<AuthApiClientOptions> authOptions,
            IProductsApiClient productsApiClient,
            IDistributedCacheManager distributedCacheManager)
        {
            _authApiClient = authApiClient;
            _authTokenStore = authTokenStore;
            _authOptions = authOptions;
            _productsApiClient = productsApiClient;
            _distributedCacheManager = distributedCacheManager;
        }

        public async Task<ProductResponse?> GetProductInfoAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            try
            {
                await CheckForAuthenticationAsync(cancellationToken);
                var productInfo = await TryGetOrCreateFromCacheAsync(productId, cancellationToken);
                return productInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task CheckForAuthenticationAsync(CancellationToken cancellationToken = default)
        {
            var token = await _authTokenStore.GetTokenAsync<UserResponse>();

            if (token == null)
            {
                var response = await _authApiClient.SendLoginAsync(new LoginUser { Email = _authOptions.CurrentValue.User, Password = _authOptions.CurrentValue.Password }, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken);
                    await _authTokenStore.SetTokenAsync(userResponse);
                }
            }
        }

        private async Task<ProductResponse?> TryGetOrCreateFromCacheAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var cachedProduct = await _distributedCacheManager.GetAsync<ProductResponse>(ProductCacheKey.GetKey(productId), cancellationToken);
            if (cachedProduct == null)
            {
                var productInfo = await _productsApiClient.GetProductAsync(productId, cancellationToken);

                if (productInfo != null)
                {
                    await _distributedCacheManager.SetAsync(ProductCacheKey.GetKey(productId), productInfo, TimeSpan.FromMinutes(15), cancellationToken);
                    return productInfo;
                }

                return default;
            }
            else
            {
                return cachedProduct;
            }
        }
    }
}
