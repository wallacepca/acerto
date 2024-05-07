using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;
using Refit;

namespace Acerto.Orders.SDK
{
    public interface IOrdersApiClient
    {
        [Get("/orders/{orderId}")]
        Task<OrderResponse> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

        [Get("/orders")]
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default);

        [Get("/orders/{page}/{pageSize}")]
        Task<PagedResult<OrderResponse>> GetOrdersAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        [Post("/orders")]
        Task<string> CreateOrderAsync(OrderRequest order, CancellationToken cancellationToken = default);
    }
}
