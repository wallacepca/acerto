using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Orders.Service.Database.Models
{
    public class OrderStatusChange : Entity
    {
        public OrderStatus Status { get; set; }
        public string? StatusReason { get; set; }
        public DateTime StatusDate { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
