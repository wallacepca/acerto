using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record OrderRequest : RequestObject
    {
        public OrderRequest(Guid id, IEnumerable<OrderItemRequest> orderItems)
            : base(id)
        {
            OrderItems = orderItems;
        }

        public IEnumerable<OrderItemRequest> OrderItems { get; private set; }
    }
}
