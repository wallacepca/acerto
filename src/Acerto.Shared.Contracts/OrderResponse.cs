using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record OrderResponse : ResponseObject
    {
        public OrderResponse(
            Guid id)
            : base(id)
        {
        }

        public IEnumerable<OrderItemResponse> OrderItems { get; set; } = null!;
        public IEnumerable<OrderStatusChangeResponse> OrderStatusChanges { get; set; } = null!;
    }
}
