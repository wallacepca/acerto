using Acerto.Products.Service.Database.Models;
using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Products.Service.Services
{
    public interface IProductsService : ICrudServiceBase<ProductRequest, ProductResponse, Product, Guid>
    {
    }
}
