using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record OrderItemRequest : RequestObject
    {
        public OrderItemRequest(Guid id, Guid productId, int quantity)
            : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
    }
}
