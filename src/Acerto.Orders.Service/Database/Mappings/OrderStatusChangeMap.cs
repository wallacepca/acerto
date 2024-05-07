using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Database.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Orders.Service.Database.Mappings
{
    public sealed class OrderStatusChangeMap : EntityMap<OrderStatusChange>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderStatusChange> builder)
        {
            base.Configure(builder);

            builder.ToTable("order_status_changes");

            builder.Property(x => x.Status)
                .HasConversion<string>();
        }
    }
}
