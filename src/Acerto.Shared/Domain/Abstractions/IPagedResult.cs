namespace Acerto.Shared.Domain.Abstractions
{
    public interface IPagedResult<T>
    {
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        int PageSize { get; set; }
        IList<T> Results { get; set; }
        long RowCount { get; set; }
    }
}
