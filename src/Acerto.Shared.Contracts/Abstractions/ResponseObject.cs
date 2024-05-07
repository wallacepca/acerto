namespace Acerto.Shared.Contracts.Abstractions
{
    public abstract record ResponseObject : ResponseObject<Guid>
    {
        protected ResponseObject(Guid id)
            : base(id)
        {
        }
    }
}
