using Acerto.Orders.Service.Consumers;
using Acerto.Orders.Service.Database;
using Acerto.Orders.Service.Database.Mappings;
using Acerto.Orders.Service.Services;
using Acerto.Shared.Controllers;
using AutoMapper;
using AutoMapper.EquivalencyExpression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderServices(this IServiceCollection services)
        {
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IProductInfoProvider, ProductInfoProvider>();
            services.AddScoped<UnitOfWorkActionFilter<OrdersDbContext>>();

            services.AddAutoMapper(
                x =>
                {
                    x.AddCollectionMappers();
                    x.UseEntityFrameworkCoreModel<OrdersDbContext>(services);
                },
                typeof(OrderModelsMappingProfile).Assembly);

            services.AddMessageConsumer<OrderPlacedConsumer>();
            services.AddMessageConsumer<OrderReceivedConsumer>();
            services.AddMessageConsumer<OrderStatusChangeConsumer>();

            return services;
        }
    }
}
