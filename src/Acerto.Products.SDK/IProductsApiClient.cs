using Acerto.Shared.Contracts;
using Acerto.Shared.Domain.Abstractions;
using Refit;

namespace Acerto.Products.SDK
{
    public interface IProductsApiClient
    {
        [Get("/products/{productId}")]
        Task<ProductResponse> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);

        [Get("/products")]
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync(CancellationToken cancellationToken = default);

        [Get("/products/{page}/{pageSize}")]
        Task<PagedResult<ProductResponse>> GetProductsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);

        [Post("/products")]
        Task<ProductResponse> CreateProductAsync(ProductRequest product, CancellationToken cancellationToken = default);

        [Put("/products")]
        Task<ProductResponse> UpdateProductAsync(ProductRequest product, CancellationToken cancellationToken = default);

        [Delete("/products/{productId}")]
        Task<int> DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
