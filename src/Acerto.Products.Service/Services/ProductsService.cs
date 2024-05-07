using Acerto.Products.Service.Database;
using Acerto.Products.Service.Database.Models;
using Acerto.Shared.Contracts;
using Acerto.Shared.Contracts.Caching;
using Acerto.Shared.Domain.Abstractions;
using Acerto.Shared.Infrastructure.Caching;
using AutoMapper;

namespace Acerto.Products.Service.Services
{
    public sealed class ProductsService : CrudServiceBase<ProductRequest, ProductResponse, Product>, IProductsService
    {
        private readonly IMapper _mapper;
        private readonly IDistributedCacheManager _distributedCacheManager;

        public ProductsService(IMapper mapper, IDistributedCacheManager distributedCacheManager, ProductsDbContext productsDbContext)
            : base(mapper, productsDbContext)
        {
            _mapper = mapper;
            _distributedCacheManager = distributedCacheManager;
        }

        // Entendo que essa implementação adicionando e removendo produtos do cache aqui tem suas limitações e complicações.
        // adicionei aqui somente para demonstração do uso, é um tema complexo e portanto demanda mais estudo antes de uma implementação correta.        pro
        public override async Task<ProductResponse?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _distributedCacheManager.GetAsync<ProductResponse>(ProductCacheKey.GetKey(id), cancellationToken);

            if (product != null)
            {
                return product;
            }

            product = await base.GetAsync(id, cancellationToken);

            if (product != null)
            {
                await _distributedCacheManager.SetAsync(ProductCacheKey.GetKey(id), product, TimeSpan.FromMinutes(15), cancellationToken);
            }

            return product;
        }

        protected override async Task OnAfterCreateEntityAsync(Product entity, CancellationToken cancellationToken)
        {
            var response = _mapper.Map<ProductResponse>(entity);
            await _distributedCacheManager.SetAsync(ProductCacheKey.GetKey(entity.Id), response, TimeSpan.FromMinutes(15), cancellationToken);
        }

        protected override async Task OnAfterUpdateEntityAsync(Product entity, CancellationToken cancellationToken)
        {
            var response = _mapper.Map<ProductResponse>(entity);
            await _distributedCacheManager.SetAsync(ProductCacheKey.GetKey(entity.Id), response, TimeSpan.FromMinutes(15), cancellationToken);
        }

        protected override async Task OnAfterDeleteEntityAsync(Guid id, CancellationToken cancellationToken)
        {
            await _distributedCacheManager.RemoveAsync(ProductCacheKey.GetKey(id), cancellationToken);
        }
    }
}
