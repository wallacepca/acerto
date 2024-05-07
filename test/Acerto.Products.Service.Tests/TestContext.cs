using Acerto.Products.Service.Database;
using Acerto.Products.Service.Database.Mappings;
using Acerto.Products.Service.Database.Models;
using AutoMapper;

namespace Acerto.Products.Service.Tests
{
    public sealed class TestContext : IDisposable
    {
        private readonly SqliteInMemoryDatabase<ProductsDbContext> _database;
        private readonly Mapper _mapper;

        private TestContext()
            : this("InMemorySharedDatabase")
        {
        }

        private TestContext(string databaseName)
        {
            var profile = new ProductModelsMappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            _mapper = new Mapper(configuration);
            _database = new SqliteInMemoryDatabase<ProductsDbContext>(databaseName);

            Scope = new TestScope(_mapper, _database);
        }

        public TestScope Scope { get; private set; }

        public static TestContext UseEmptyDb() => new(Guid.NewGuid().ToString());
        public static TestContext UseSharedDb() => new();

        public static IEnumerable<Product> GetSampleData() => [
            new Product("Samsung Galaxy S24 Ultra", "Samsung", 5_000)
            {
                Id = Guid.Parse("220c2a04-9da6-4fc1-b909-7cba5baa1f70")
            },
            new Product("Iphone 15 Pro Max", "Apple", 10_000)
            {
                Id = Guid.Parse("adbca8e5-832c-45c3-97ad-46fdf3c30349")
            }
            ];

        public TestScope UseNewScope()
        {
            Scope = new TestScope(_mapper, _database);
            return Scope;
        }

        public async Task<int> SeedDataAsync(Product product) => await SeedDataAsync([product]);

        public async Task<int> SeedDataAsync(IEnumerable<Product> products)
        {
            using var seedScope = UseNewScope();
            await seedScope.DbContext.Set<Product>().AddRangeAsync(products);
            return await seedScope.DbContext.SaveChangesAsync();
        }

        public async Task<int> SeedSampleDataAsync()
            => await SeedDataAsync(GetSampleData());

        public void Dispose()
        {
            Scope.Dispose();
            _database?.Dispose();
        }
    }
}
