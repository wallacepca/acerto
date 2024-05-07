using System.Text.Json;
using Acerto.Orders.Service.Database.Models;
using Acerto.Shared.Contracts;
using Acerto.Shared.Database.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Orders.Service.Database.Mappings
{
    public sealed class OrderItemMap : EntityMap<OrderItem>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("order_items", x =>
            {
                x.HasCheckConstraint("order_items_quantity_greather_than_or_equal_1", "quantity >= 1");
            });

            builder.Property(x => x.Item)
                .HasConversion(
                    x => JsonSerializer.Serialize(x, default(JsonSerializerOptions)),
                    x => JsonSerializer.Deserialize<ProductResponse>(x, default(JsonSerializerOptions)))
                .HasColumnType("jsonb");
            builder.HasIndex(x => new { x.OrderId, x.ProductId })
                .IsUnique();
        }
    }
}
