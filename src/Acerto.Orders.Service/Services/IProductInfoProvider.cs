using Acerto.Shared.Contracts;

namespace Acerto.Orders.Service.Services
{
    public interface IProductInfoProvider
    {
        Task<ProductResponse?> GetProductInfoAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
