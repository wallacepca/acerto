using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHost UpdateDatabase<TDbContext>(this IHost host)
        where TDbContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                scope.ServiceProvider.UpdateDatabase<TDbContext>();
            }

            return host;
        }

        private static void UpdateDatabase<TDbContext>(this IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            ILogger logger = serviceProvider.GetRequiredService<ILogger<TDbContext>>();

            using var scope = serviceProvider.CreateScope();
            using var appContext = serviceProvider.GetRequiredService<TDbContext>();

            try
            {
                appContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating database");

                throw;
            }
        }
    }
}
