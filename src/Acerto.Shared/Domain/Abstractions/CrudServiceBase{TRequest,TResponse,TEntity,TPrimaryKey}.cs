using Acerto.Shared.Contracts.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Shared.Domain.Abstractions
{
    public abstract class CrudServiceBase<TRequest, TResponse, TEntity, TPrimaryKey>
        : ICrudServiceBase<TRequest, TResponse, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TRequest : IRequestObject<TPrimaryKey>
        where TResponse : IResponseObject<TPrimaryKey>
    {
        private readonly IMapper _mapper;

        protected CrudServiceBase(IMapper mapper, DbContext dbContext)
        {
            _mapper = mapper;
            Context = dbContext;
        }

        protected DbContext Context { get; set; }

        public virtual async Task<TResponse?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var entity = await GetEntityAsync(id, cancellationToken);
            return _mapper.Map<TResponse?>(entity);
        }

        public virtual async Task<IEnumerable<TResponse>?> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await Context.Set<TEntity>().ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TResponse>>(entities);
        }

        public virtual async Task<IPagedResult<TResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = Context.Set<TEntity>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

            var rowCount = await Context.Set<TEntity>().LongCountAsync(cancellationToken);
            var items = _mapper.Map<IList<TResponse>>(query);

            return new PagedResult<TResponse>(items, page, pageSize, rowCount);
        }

        public virtual async Task<TResponse?> CreateAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<TEntity>(request);

            if (entity != null)
            {
                await OnBeforeCreateAsync(request, entity, cancellationToken);
                await CreateEntityAsync(entity, cancellationToken);
                var response = _mapper.Map<TResponse>(entity);
                return response;
            }

            return default;
        }

        public virtual async Task<TEntity> CreateEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
            await OnAfterCreateEntityAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<TResponse?> UpdateAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await GetEntityAsync(request.Id, cancellationToken);

            if (entity != null)
            {
                await OnBeforeUpdateAsync(request, entity);
                _mapper.Map(request, entity);
                await UpdateEntityAsync(entity, cancellationToken);
                var response = _mapper.Map<TResponse>(entity);
                return response;
            }

            return default;
        }

        public virtual async Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().Update(entity);
            await OnAfterCreateEntityAsync(entity, cancellationToken);
            return await Task.FromResult(entity);
        }

        public virtual async Task<TResponse?> CreateOrUpdateAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Id?.Equals(default(TPrimaryKey)) ?? true)
            {
                return await CreateAsync(request, cancellationToken);
            }
            else
            {
                var response = await UpdateAsync(request, cancellationToken);

                if (response == null)
                {
                    return await CreateAsync(request, cancellationToken);
                }

                return response;
            }
        }

        public virtual async Task<int> DeleteEntityAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var affectedRows = await Context.Set<TEntity>()
                 .Where(x => x.Id != null && x.Id.Equals(id))
                 .ExecuteDeleteAsync(cancellationToken);

            if (affectedRows > 0)
            {
                await OnAfterDeleteEntityAsync(id, cancellationToken);
            }

            return affectedRows;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetEntityAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var result = await Context.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);

            return result;
        }

        protected virtual async Task OnBeforeCreateAsync(TRequest request, TEntity entity, CancellationToken cancellationToken = default) => await Task.CompletedTask;

        protected virtual async Task OnBeforeUpdateAsync(TRequest request, TEntity oldEntity) => await Task.CompletedTask;

        protected virtual async Task OnAfterCreateEntityAsync(TEntity entity, CancellationToken cancellationToken) => await Task.CompletedTask;

        protected virtual async Task OnAfterUpdateEntityAsync(TEntity entity, CancellationToken cancellationToken) => await Task.CompletedTask;

        protected virtual async Task OnAfterDeleteEntityAsync(TPrimaryKey id, CancellationToken cancellationToken) => await Task.CompletedTask;
    }
}
