using Acerto.Orders.Service.Database;
using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;
using AutoMapper;

namespace Acerto.Orders.Service.Services
{
    public sealed class OrdersService : CrudServiceBase<OrderRequest, OrderResponse, Order>, IOrdersService
    {
        private readonly IMapper _mapper;

        public OrdersService(IMapper mapper, OrdersDbContext ordersDbContext)
            : base(mapper, ordersDbContext)
        {
            _mapper = mapper;
        }

        public async Task<OrderResponse> ChangeOrderStatusAsync(Guid orderId, OrderStatus orderStatus, string? statusReason = null, CancellationToken cancellationToken = default)
        {
            var order = await GetEntityAsync(orderId, cancellationToken);

            if (order != null)
            {
                order.ChangeStatus(orderStatus, statusReason);
                return _mapper.Map<OrderResponse>(order);
            }

            throw new Exception($"Order with id {orderId} not found.");
        }

        protected override Task OnBeforeCreateAsync(OrderRequest request, Order entity, CancellationToken cancellationToken = default)
        {
            if (entity.OrderStatusChanges.Count == 0)
            {
                entity.OrderStatusChanges.Add(new OrderStatusChange { Status = OrderStatus.Received, StatusDate = DateTime.UtcNow });
            }

            return base.OnBeforeCreateAsync(request, entity, cancellationToken);
        }
    }
}
