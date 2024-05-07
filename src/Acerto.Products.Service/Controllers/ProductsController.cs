using Acerto.Products.Service.Database.Models;
using Acerto.Products.Service.Services;
using Acerto.Products.Service.Validations;
using Acerto.Shared.Contracts;
using Acerto.Shared.Controllers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acerto.Products.Service.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public sealed class ProductsController : CrudControllerBase<ProductRequest, ProductResponse, Product>
    {
        public ProductsController(
            IProductsService productsService)
            : base(productsService)
        {
        }

        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status422UnprocessableEntity)]
        public override async Task<ActionResult<ProductResponse>> PostAsync(ProductRequest request, CancellationToken cancellationToken = default)
        {
            // fluent validation poderia ser melhor integrado ao DI/pipeline de validação.
            // Porém como quis utilizá-lo, por considerá-lo melhor e mais flexivel que dataannotations,
            // e como nao há tempo suficiente para focar em detalhes, como um objeto de retorno apropriado, mais limpo, preferi dar prioridade a outras coisas.
            var validator = new CreateProductValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                return UnprocessableEntity(result);
            }

            return await base.PostAsync(request, cancellationToken);
        }

        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status422UnprocessableEntity)]
        public override async Task<ActionResult<ProductResponse>> PutAsync(ProductRequest request, CancellationToken cancellationToken = default)
        {
            var validator = new UpdateProductValidator();
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                return UnprocessableEntity(result);
            }

            return await base.PutAsync(request, cancellationToken);
        }
    }
}
