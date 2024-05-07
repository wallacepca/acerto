namespace Acerto.Shared.Domain.Abstractions
{
    public sealed class PagedResult<T> : IPagedResult<T>
    {
        public PagedResult(IList<T> results, int currentPage, int pageSize, long rowCount)
        {
            Results = results ?? [];
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = rowCount;
            PageCount = (int)Math.Ceiling(Convert.ToDouble(RowCount) / Convert.ToDouble(pageSize));
        }

        public IList<T> Results { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public long RowCount { get; set; }
    }
}
