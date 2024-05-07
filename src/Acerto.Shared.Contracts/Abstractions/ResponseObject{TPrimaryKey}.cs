namespace Acerto.Shared.Contracts.Abstractions
{
    public abstract record ResponseObject<TPrimaryKey>
        : IResponseObject<TPrimaryKey>
    {
        protected ResponseObject(TPrimaryKey id)
        {
            Id = id;
        }

        public TPrimaryKey Id { get; set; }
    }
}
