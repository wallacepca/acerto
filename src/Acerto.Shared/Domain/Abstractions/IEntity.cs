namespace Acerto.Shared.Domain.Abstractions
{
    public interface IEntity : IEntity<Guid>
    {
    }

    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; }
    }
}
