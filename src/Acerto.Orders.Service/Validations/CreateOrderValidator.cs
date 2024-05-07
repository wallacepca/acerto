using Acerto.Shared.Contracts;
using FluentValidation;

namespace Acerto.Orders.Service.Validations
{
    public sealed class CreateOrderValidator : AbstractValidator<OrderRequest>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.OrderItems)
                .NotEmpty();

            RuleForEach(x => x.OrderItems)
                .SetValidator(new CreateOrderItemValidator());
        }
    }
}
