namespace Acerto.Shared.Domain.Abstractions
{
    public abstract class Entity : Entity<Guid>, IEntity
    {
        public Entity()
         : this(default)
        {
        }

        protected Entity(Guid id)
            : base(id)
        {
        }
    }
}
