using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Orders.Service.Database.Models
{
    public class OrderItem : Entity
    {
        public OrderItem(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual ProductResponse? Item { get; set; } = null!;
    }
}
