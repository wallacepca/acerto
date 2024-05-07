using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Database.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Orders.Service.Database.Mappings
{
    public sealed class OrderMap : EntityMap<Order>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.ToTable("orders");

            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);

            builder.HasMany(x => x.OrderStatusChanges)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
        }
    }
}
