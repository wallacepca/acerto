using Acerto.Products.SDK;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProductsApiClientExtensions
    {
        public static IServiceCollection AddProductsSDK(this IServiceCollection services, Action<ProductsApiClientOptions> configureOptions)
            => services.AddSDK<IProductsApiClient, ProductsApiClientOptions>(configureOptions);
    }
}
