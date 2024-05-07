using Acerto.Shared.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acerto.Shared.Database.Abstractions
{
    public abstract class EntityMap<T> : EntityMap<T, Guid>
        where T : class, IEntity<Guid>
    {
    }

#pragma warning disable SA1402 // File may only contain a single type
    public abstract class EntityMap<T, TPrimaryKey> : IEntityTypeConfiguration<T>
#pragma warning restore SA1402 // File may only contain a single type
        where T : class, IEntity<TPrimaryKey>
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.HasKey(x => x.Id);
        }
    }
}
