using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acerto.Shared.Controllers
{
    public sealed class UnitOfWorkActionFilter<TDbContext> : IAsyncActionFilter
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly ILogger<UnitOfWorkActionFilter<TDbContext>> _logger;

        public UnitOfWorkActionFilter(TDbContext dbContext, ILogger<UnitOfWorkActionFilter<TDbContext>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next.Invoke();

            if (executedContext.Exception == null)
            {
                var savedItemsCount = await _dbContext.SaveChangesAsync();
                if (savedItemsCount > 0)
                {
                    _logger.LogDebug("{savedItemsCount} items saved in Database", savedItemsCount);
                }
            }
        }
    }
}
