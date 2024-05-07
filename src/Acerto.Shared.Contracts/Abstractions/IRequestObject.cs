namespace Acerto.Shared.Contracts.Abstractions
{
    public interface IRequestObject<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
