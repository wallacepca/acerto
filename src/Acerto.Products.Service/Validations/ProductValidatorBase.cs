using Acerto.Shared.Contracts;
using FluentValidation;

namespace Acerto.Products.Service.Validations
{
    public abstract class ProductValidatorBase : AbstractValidator<ProductRequest>
    {
        public ProductValidatorBase()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Brand)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.Color)
                .MaximumLength(255);

            RuleFor(x => x.Description)
                .MaximumLength(65535);
        }
    }
}
