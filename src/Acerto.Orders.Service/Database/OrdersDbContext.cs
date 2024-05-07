using Acerto.Orders.Service.Database.Mappings;
using Acerto.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Orders.Service.Database
{
    public sealed class OrdersDbContext : DbContextBase
    {
        public OrdersDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderMap).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
