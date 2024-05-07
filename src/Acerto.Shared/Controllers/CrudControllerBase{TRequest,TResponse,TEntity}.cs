using Acerto.Shared.Domain.Abstractions;

namespace Acerto.Shared.Controllers
{
    public abstract class CrudControllerBase<TRequest, TResponse, TEntity>
        : CrudControllerBase<TRequest, TResponse, TEntity, Guid>
        where TEntity : class
    {
        public CrudControllerBase(ICrudServiceBase<TRequest, TResponse, TEntity, Guid> service)
            : base(service)
        {
        }
    }
}
