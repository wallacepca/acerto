namespace Acerto.Orders.Service.Database.Models
{
    public sealed class OrderStatusChangeComparer : IComparer<OrderStatusChange>
    {
        public int Compare(OrderStatusChange? x, OrderStatusChange? y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x is not null && y is not null)
            {
                if (x.StatusDate > y.StatusDate)
                {
                    return 1;
                }
            }

            return -1;
        }
    }
}
