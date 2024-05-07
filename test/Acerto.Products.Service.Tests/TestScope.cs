using Acerto.Products.Service.Controllers;
using Acerto.Products.Service.Database;
using Acerto.Products.Service.Services;
using Acerto.Shared.Infrastructure.Caching;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;

namespace Acerto.Products.Service.Tests
{
    public sealed class TestScope : IDisposable
    {
        public TestScope(IMapper mapper, SqliteInMemoryDatabase<ProductsDbContext> database)
        {
            DbContext = database.GetContext();
            var distributedCache = Substitute.For<IDistributedCache>();
            var distributedCacheManager = new DistributedCacheManager(distributedCache);
            var productsService = new ProductsService(mapper, distributedCacheManager, DbContext);
            Controller = new ProductsController(productsService);
        }

        public ProductsDbContext DbContext { get; private set; }
        public ProductsController Controller { get; private set; }

        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
