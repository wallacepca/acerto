namespace Acerto.Shared.Contracts.Abstractions
{
    public abstract record RequestObject<TPrimaryKey> : IRequestObject<TPrimaryKey>
    {
        public RequestObject(TPrimaryKey id)
        {
            Id = id;
        }

        public virtual TPrimaryKey Id { get; set; }
    }
}
