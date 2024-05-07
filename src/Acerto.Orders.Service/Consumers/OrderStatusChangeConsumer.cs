using Acerto.Orders.Service.Services;
using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Events;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;

namespace Acerto.Orders.Service.Consumers
{
    /// <summary>
    /// Código criado para simular uma maquina de estados dos pedidos, durante seu processamento,
    /// e continuamente atualizar o status do pedido até que este seja finalizado.
    /// Este é código que nao deveria estar localizado dentro de uma classe que consome mensagens, porem sua finalidade aqui é somente para demonstração.
    /// </summary>
    public sealed class OrderStatusChangeConsumer : IMessageConsumer<OrderStatusChanged>
    {
        private readonly IOrdersService _ordersService;
        private readonly IProductInfoProvider _productInfoProvider;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<OrderStatusChangeConsumer> _logger;

        public OrderStatusChangeConsumer(
            IOrdersService ordersService,
            IProductInfoProvider productInfoProvider,
            IServiceBus serviceBus,
            ILogger<OrderStatusChangeConsumer> logger)
        {
            _ordersService = ordersService;
            _productInfoProvider = productInfoProvider;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChanged message)
        {
            _logger.LogInformation("{messageType} message consumed for order {orderId} at time {time}.", nameof(OrderStatusChanged), message.Order.Id, DateTime.UtcNow);

            var orderEntity = await _ordersService.GetEntityAsync(message.Order.Id);

            if (orderEntity != null)
            {
                var latestStatus = message.Order.OrderStatusChanges.LastOrDefault();
                if (latestStatus is not null)
                {
                    if (latestStatus.Status != OrderStatus.Processing
                        && latestStatus.Status != OrderStatus.Completed
                        && latestStatus.Status != OrderStatus.Canceled)
                    {
                        // espera propositalmente 15s antes de alterar o status, para que dê tempo de perceber a troca de status ao consultar api ou db
                        await Task.Delay(15_000);

                        var newStatus = latestStatus.Status;

                        switch (latestStatus.Status)
                        {
                            case OrderStatus.Received:
                                newStatus = OrderStatus.Processing;
                                break;
                            case OrderStatus.PaymentPending:
                                newStatus = OrderStatus.PaymentCompleted;
                                break;
                            case OrderStatus.PaymentCompleted:
                                newStatus = OrderStatus.Shipped;
                                break;
                            case OrderStatus.Shipped:
                                newStatus = OrderStatus.Completed;
                                break;
                            case OrderStatus.Completed:
                                break;
                            default:
                                break;
                        }

                        var order = await _ordersService.ChangeOrderStatusAsync(message.Order.Id, newStatus);
                        await _ordersService.SaveChangesAsync();

                        _logger.LogInformation("order {orderId} have its status changed from {oldStatus} to {newStatus} at {time}.", message.Order.Id, latestStatus.Status, newStatus, DateTime.UtcNow);

                        await _serviceBus.PublishAsync(new OrderStatusChanged(order));
                    }
                    else if (latestStatus.Status == OrderStatus.Canceled || latestStatus.Status == OrderStatus.Completed)
                    {
                        _logger.LogInformation("order {orderId} finished with status {status} at {time}", message.Order.Id, latestStatus.Status, DateTime.UtcNow);
                    }
                }
            }
        }
    }
}
