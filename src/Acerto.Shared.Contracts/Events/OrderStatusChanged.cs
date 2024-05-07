namespace Acerto.Shared.Contracts.Events
{
    public record OrderStatusChanged
    {
        public OrderStatusChanged(OrderResponse order)
        {
            Order = order;
        }

        public OrderResponse Order { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
    }
}
