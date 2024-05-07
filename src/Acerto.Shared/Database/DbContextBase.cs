using System.Diagnostics.CodeAnalysis;
using Acerto.Shared.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Acerto.Shared.Database
{
    public abstract class DbContextBase : DbContext
    {
        protected DbContextBase([NotNull] DbContextOptions options)
            : base(options)
        {
        }

        public override int SaveChanges()
        {
            CheckEntitiesBeforeSave();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CheckEntitiesBeforeSave();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void CheckEntitiesBeforeSave()
        {
            var currentDate = DateTime.UtcNow;
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IHasCreationDate creationDateEntity)
                    {
                        creationDateEntity.CreatedAt = currentDate;
                    }

                    if (entry.Entity is IHasUpdateDate updateDateEntity)
                    {
                        updateDateEntity.UpdatedAt = currentDate;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IHasUpdateDate updateDateEntity)
                    {
                        updateDateEntity.UpdatedAt = currentDate;
                    }
                }
            }
        }
    }
}
