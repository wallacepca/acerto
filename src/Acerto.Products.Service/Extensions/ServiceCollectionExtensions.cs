using Acerto.Products.Service.Database;
using Acerto.Products.Service.Database.Mappings;
using Acerto.Products.Service.Services;
using Acerto.Shared.Controllers;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddTransient<IProductsService, ProductsService>();
            services.AddScoped<UnitOfWorkActionFilter<ProductsDbContext>>();

            services.AddAutoMapper(
                x =>
                {
                    x.AddCollectionMappers();
                    x.UseEntityFrameworkCoreModel<ProductsDbContext>(services);
                },
                typeof(ProductModelsMappingProfile).Assembly);

            return services;
        }
    }
}
