using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Orders.Service.Services
{
    public interface IOrdersService : ICrudServiceBase<OrderRequest, OrderResponse, Order, Guid>
    {
        Task<OrderResponse> ChangeOrderStatusAsync(Guid orderId, OrderStatus orderStatus, string? statusReason = null, CancellationToken cancellationToken = default);
    }
}
