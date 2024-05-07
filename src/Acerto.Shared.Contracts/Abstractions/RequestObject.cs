namespace Acerto.Shared.Contracts.Abstractions
{
    public abstract record RequestObject : RequestObject<Guid>
    {
        protected RequestObject(Guid id)
            : base(id)
        {
        }
    }
}
