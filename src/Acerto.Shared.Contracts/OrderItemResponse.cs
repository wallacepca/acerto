using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record OrderItemResponse : RequestObject
    {
        public OrderItemResponse(
            Guid id,
            Guid productId,
            int quantity,
            ProductResponse item)
            : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
            Item = item;
        }

        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public ProductResponse Item { get; private set; }
    }
}
