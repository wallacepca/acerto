using FluentValidation;

namespace Acerto.Products.Service.Validations
{
    public sealed class UpdateProductValidator : ProductValidatorBase
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
