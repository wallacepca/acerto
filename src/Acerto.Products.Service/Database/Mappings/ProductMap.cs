using Acerto.Products.Service.Database.Models;
using Acerto.Shared.Database.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Products.Service.Database.Mappings
{
    public sealed class ProductMap : EntityMap<Product>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.ToTable(
                "products",
                x =>
                {
                    x.HasCheckConstraint("products_price_greather_than_0", "price > 0");
                });

            builder.HasIndex(t => t.Name)
                .IsUnique();

            builder.Property(x => x.Name)
                .HasMaxLength(255);

            builder.Property(x => x.Description)
                .HasMaxLength(65535);

            builder.Property(x => x.Color)
                .HasMaxLength(255);
        }
    }
}
