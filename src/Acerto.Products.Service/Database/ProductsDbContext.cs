using Acerto.Products.Service.Database.Mappings;
using Acerto.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Products.Service.Database
{
    public sealed class ProductsDbContext : DbContextBase
    {
        public ProductsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductMap).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
