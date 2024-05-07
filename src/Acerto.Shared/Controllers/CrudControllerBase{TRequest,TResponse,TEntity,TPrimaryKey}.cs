using Acerto.Shared.Contracts.Messages;
using Acerto.Shared.Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Acerto.Shared.Controllers
{
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public abstract class CrudControllerBase<TRequest, TResponse, TEntity, TPrimaryKey>
        : ControllerBase
        where TEntity : class
    {
        private readonly ICrudServiceBase<TRequest, TResponse, TEntity, TPrimaryKey> _service;

        public CrudControllerBase(ICrudServiceBase<TRequest, TResponse, TEntity, TPrimaryKey> service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<TResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{page:int:min(1)}/{pageSize:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<ActionResult<IPagedResult<TResponse>>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page == 0 || pageSize == 0)
            {
                return BadRequest(ResultMessages.PageAndPageSizeErrorMessage);
            }

            var response = await _service.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<TResponse>> GetByIdAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var response = await _service.GetAsync(id, cancellationToken);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Produces("application/json")]
        public virtual async Task<ActionResult<TResponse>> PostAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            return await PostOrPutAsync(request, cancellationToken);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public virtual async Task<ActionResult<TResponse>> PutAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            return await PostOrPutAsync(request, cancellationToken);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<int>> DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var response = await _service.DeleteEntityAsync(id, cancellationToken);

            if (response == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        private async Task<ActionResult<TResponse>> PostOrPutAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var response = await _service.CreateOrUpdateAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
