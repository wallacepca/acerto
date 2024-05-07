using Acerto.Orders.Service.Services;
using Acerto.Orders.Service.Validations;
using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Events;
using Acerto.Shared.Contracts.Messages;
using Acerto.Shared.Domain.Abstractions;
using Acerto.Shared.Infrastructure.ServiceBus.Abstractions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acerto.Orders.Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrdersService _orderService;
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrdersService orderService, IServiceBus serviceBus, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _serviceBus = serviceBus;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _orderService.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{page:int:min(1)}/{pageSize:int:min(1)}")]
        [ProducesResponseType(typeof(IPagedResult<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var response = await _orderService.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _orderService.GetAsync(id, cancellationToken);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status422UnprocessableEntity)]
        [Produces("application/json")]
        public async Task<IActionResult> PostAsync(OrderRequest request, CancellationToken cancellationToken = default)
        {
            /* Intencionalmente aceitarei pedidos mesmo sem validar os produtos neste momento,
               isso será feito mais tarde durante o processamento da fila, e em caso do produto nao existir o pedido será automaticamente cancelado.*/

            var validator = new CreateOrderValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            // fluent validation poderia ser melhor integrado ao DI/pipeline de validação.
            // Porém como quis utilizá-lo, por considerá-lo melhor e mais flexivel que dataannotations,
            // e como nao há tempo suficiente para focar em detalhes, como um objeto de retorno apropriado, mais limpo, preferi dar prioridade a outras coisas.

            if (validationResult.IsValid)
            {
                _logger.LogInformation("Order {orderId} placed.", request.Id);

                // publica mensagem no rabbitmq para processamento posterior
                await _serviceBus.PublishAsync(new OrderPlaced(request), cancellationToken: cancellationToken);

                _logger.LogInformation("Order {orderId} enqueued for late processing.", request.Id);

                return Ok(ResultMessages.OrderPlacedSuccessfully);
            }

            return UnprocessableEntity(validationResult);
        }
    }
}
