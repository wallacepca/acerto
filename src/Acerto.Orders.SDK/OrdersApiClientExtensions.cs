using Acerto.Orders.SDK;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrdersApiClientExtensions
    {
        public static IServiceCollection AddOrdersSDK(this IServiceCollection services, Action<OrdersApiClientOptions> configureOptions)
            => services.AddSDK<IOrdersApiClient, OrdersApiClientOptions>(configureOptions);
    }
}
