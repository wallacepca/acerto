using Acerto.Shared.Contracts.Abstractions;

namespace Acerto.Shared.Contracts
{
    public record OrderStatusChangeResponse : RequestObject
    {
        public OrderStatusChangeResponse(Guid id)
            : base(id)
        {
        }

        public OrderStatus Status { get; set; }
        public string? StatusReason { get; set; }
        public DateTime StatusDate { get; set; }
    }
}
