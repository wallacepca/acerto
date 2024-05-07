using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Orders.Service.Database.Models
{
    public class Order : Entity, IHasCreationDate, IHasUpdateDate
    {
        public Order()
        {
            OrderStatusChanges = new SortedSet<OrderStatusChange>(new OrderStatusChangeComparer());
        }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;
        public virtual ICollection<OrderStatusChange> OrderStatusChanges { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public void ChangeStatus(OrderStatus orderStatus, string? statusReason = null)
        {
            OrderStatusChanges.Add(new OrderStatusChange { Status = orderStatus, StatusDate = DateTime.UtcNow, StatusReason = statusReason });
        }
    }
}
