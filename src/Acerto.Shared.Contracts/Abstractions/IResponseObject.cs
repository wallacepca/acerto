namespace Acerto.Shared.Contracts.Abstractions
{
    public interface IResponseObject<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}
