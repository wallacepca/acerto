using Acerto.Shared.Contracts.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Shared.Domain.Abstractions
{
    public abstract class CrudServiceBase<TRequest, TResponse, TEntity>
        : CrudServiceBase<TRequest, TResponse, TEntity, Guid>,
        ICrudServiceBase<TRequest, TResponse, TEntity, Guid>
        where TEntity : class, IEntity<Guid>
        where TRequest : IRequestObject<Guid>
        where TResponse : IResponseObject<Guid>
    {
        protected CrudServiceBase(IMapper mapper, DbContext dbContext)
            : base(mapper, dbContext)
        {
        }
    }
}
