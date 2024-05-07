namespace Acerto.Shared.Domain.Abstractions
{
    public interface IHasUpdateDate
    {
        DateTime? UpdatedAt { get; set; }
    }
}
