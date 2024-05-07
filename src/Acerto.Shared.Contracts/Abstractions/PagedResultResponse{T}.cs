namespace Acerto.Shared.Contracts.Abstractions
{
    public class PagedResultResponse<T>
    {
        private int _currentPage;

        public PagedResultResponse()
        {
            TotalRecords = 10;
            CurrentPage = 1;
            PageSize = 10;
            Items = [];
        }

        public PagedResultResponse(IEnumerable<T> items, int page, int pageSize, int totalRecords)
        {
            TotalRecords = totalRecords;
            PageSize = pageSize == 0 ? 10 : pageSize;
            PageCount = GetPageCount();
            CurrentPage = page;
            NextPage = CurrentPage >= PageCount ? PageCount : CurrentPage + 1;
            PreviousPage = CurrentPage <= 1 ? 1 : CurrentPage - 1;

            Items = items?.Cast<T>() ?? [];
        }

        public int FirstPage => 1;

        public int LastPage => PageCount;

        public int TotalRecords { get; set; }
        public int PreviousPage { get; set; }
        public int CurrentPage
        {
            get
            {
                if (_currentPage <= 1)
                {
                    _currentPage = 1;
                }
                else if (_currentPage > PageCount)
                {
                    _currentPage = PageCount;
                }

                return _currentPage;
            }
            set => _currentPage = value;
        }

        public int NextPage { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }

        public IEnumerable<T> Items { get; set; }

        private int GetPageCount()
        {
            if (TotalRecords % PageSize == 0)
            {
                if (TotalRecords == 0)
                {
                    return 1;
                }

                return TotalRecords / PageSize;
            }
            else
            {
                return (TotalRecords / PageSize) + 1;
            }
        }
    }
}
