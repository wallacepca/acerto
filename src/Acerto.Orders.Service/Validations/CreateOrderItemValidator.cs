using Acerto.Shared.Contracts;
using FluentValidation;

namespace Acerto.Orders.Service.Validations
{
    public sealed class CreateOrderItemValidator : AbstractValidator<OrderItemRequest>
    {
        public CreateOrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1);
        }
    }
}
