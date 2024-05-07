namespace Acerto.Shared.Domain.Abstractions
{
    public interface ICrudServiceBase<TRequest, TResponse, TEntity>
        : ICrudServiceBase<TRequest, TResponse, TEntity, Guid>
        where TEntity : class
    {
    }

    public interface ICrudServiceBase<TRequest, TResponse, TEntity, TPrimaryKey>
        : IService
        where TEntity : class
    {
        Task<TResponse?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TResponse>?> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IPagedResult<TResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<TResponse?> CreateAsync(TRequest request, CancellationToken cancellationToken = default);
        Task<TResponse?> CreateOrUpdateAsync(TRequest request, CancellationToken cancellationToken = default);
        Task<TResponse?> UpdateAsync(TRequest request, CancellationToken cancellationToken = default);
        Task<int> DeleteEntityAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<TEntity> CreateEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity?> GetEntityAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
    }
}
