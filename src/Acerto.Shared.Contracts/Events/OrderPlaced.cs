namespace Acerto.Shared.Contracts.Events
{
    public record OrderPlaced
    {
        public OrderPlaced(OrderRequest order)
        {
            Order = order;
        }

        public OrderRequest Order { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
    }
}
