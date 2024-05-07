using Acerto.Orders.Service.Services;
using Acerto.Shared.Contracts.Events;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;

namespace Acerto.Orders.Service.Consumers
{
    /// <summary>
    /// Este consumidor irá somente registrar o pedido, sem se preocupar com nenhuma validação.
    /// </summary>
    public sealed class OrderPlacedConsumer : IMessageConsumer<OrderPlaced>
    {
        private readonly IOrdersService _ordersService;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<OrderPlacedConsumer> _logger;

        public OrderPlacedConsumer(IOrdersService ordersService, IServiceBus serviceBus, ILogger<OrderPlacedConsumer> logger)
        {
            _ordersService = ordersService;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task Handle(OrderPlaced message)
        {
            _logger.LogInformation("Order {orderId} dequeued.", message.Order.Id);

            try
            {
                if (message.Order.Id != default)
                {
                    var existingOrder = await _ordersService.GetEntityAsync(message.Order.Id);
                    if (existingOrder != null)
                    {
                        var latestStatus = existingOrder.OrderStatusChanges.LastOrDefault();

                        if (latestStatus != null
                            && latestStatus.Status != Shared.Contracts.OrderStatus.Received)
                        {
                            // caso um pedido com mesmo id seja submetido novamente este nao será processado.
                            // o ideal é tratar o problema na fonte com uma validação mais consistente, então entendo que esse é um caso que embora deva ser levado em conta,
                            // pelas mesmas razões somente será ignorado aqui para evitar problemas.
                            _logger.LogInformation("Order {orderId} ignored because there's another order with same id.", message.Order.Id);

                            return;
                        }
                    }
                }

                var order = await _ordersService.CreateAsync(message.Order);
                await _ordersService.SaveChangesAsync();

                if (order != null)
                {
                    _logger.LogInformation("Order {orderId} received at {time}.", order.Id, DateTime.UtcNow);
                    await _serviceBus.PublishAsync(new OrderStatusChanged(order));
                    _logger.LogInformation("{messageType} message published for order {orderId} at {time}.", nameof(OrderStatusChanged), order.Id, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error saving order {orderId} at {time}.", message.Order.Id, DateTime.UtcNow);

                throw;
            }
        }
    }
}
