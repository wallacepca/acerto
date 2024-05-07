using Acerto.Orders.Service.Services;
using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Events;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;

namespace Acerto.Orders.Service.Consumers
{
    public sealed class OrderReceivedConsumer : IMessageConsumer<OrderStatusChanged>
    {
        private readonly IOrdersService _ordersService;
        private readonly IProductInfoProvider _productInfoProvider;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<OrderReceivedConsumer> _logger;

        public OrderReceivedConsumer(
            IOrdersService ordersService,
            IProductInfoProvider productInfoProvider,
            IServiceBus serviceBus,
            ILogger<OrderReceivedConsumer> logger)
        {
            _ordersService = ordersService;
            _productInfoProvider = productInfoProvider;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChanged message)
        {
            var orderEntity = await _ordersService.GetEntityAsync(message.Order.Id);

            if (orderEntity != null)
            {
                var latestStatus = message.Order.OrderStatusChanges.LastOrDefault();

                if (latestStatus != null
                    && latestStatus.Status == Shared.Contracts.OrderStatus.Processing)
                {
                    _logger.LogInformation("Starting processing order {orderId} at {time}.", message.Order.Id, DateTime.UtcNow);

                    try
                    {
                        var products = new Dictionary<Guid, ProductResponse?>();
                        foreach (var item in orderEntity.OrderItems)
                        {
                            try
                            {
                                var productInfo = await _productInfoProvider.GetProductInfoAsync(item.ProductId);

                                if (productInfo != null)
                                {
                                    item.Item = productInfo;
                                }

                                products.Add(item.ProductId, productInfo);
                            }

                            // Por razoes de demonstração o pedido será cancelado em caso de qualquer problema.
                            catch
                            {
                                throw;
                            }
                        }

                        if (products.Count != 0)
                        {
                            // altera para o status seguinte, depois de processar as informações dos produtos.
                            orderEntity = await _ordersService.UpdateEntityAsync(orderEntity);
                            var orderResponse = await _ordersService.ChangeOrderStatusAsync(message.Order.Id, OrderStatus.PaymentPending);
                            await _ordersService.SaveChangesAsync();
                            await _serviceBus.PublishAsync(new OrderStatusChanged(orderResponse));
                        }
                    }

                    // Por razoes de demonstração o pedido será cancelado em caso de qualquer problema.
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Error processing order {orderId} at {time}.", message.Order.Id, DateTime.UtcNow);
                        var orderResponse = await _ordersService.ChangeOrderStatusAsync(message.Order.Id, OrderStatus.Canceled, statusReason: ex.Message);
                        await _ordersService.SaveChangesAsync();
                        await _serviceBus.PublishAsync(new OrderStatusChanged(orderResponse));
                    }
                }
            }
        }
    }
}
