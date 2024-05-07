namespace Acerto.Shared.Domain.Abstractions
{
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        protected Entity(TPrimaryKey id)
        {
            Id = id;
        }

        public virtual TPrimaryKey Id { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}";
        }
    }
}
