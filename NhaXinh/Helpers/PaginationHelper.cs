namespace NhaXinh.Helpers
{
    public static class PaginationHelper
    {
        public static int TotalPages(int totalCount, int pageSize)
        {
            if (pageSize <= 0) return 1;
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public static int Clamp(int page, int totalPages)
            => Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
        public static int Skip(int page, int pageSize)
            => (Math.Max(1, page) - 1) * pageSize;
        public static List<int> PageNumbers(int currentPage, int totalPages, int delta = 2)
        {
            int start = Math.Max(1, currentPage - delta);
            int end = Math.Min(totalPages, currentPage + delta);
            return Enumerable.Range(start, end - start + 1).ToList();
        }

        public static bool HasPrevious(int currentPage) => currentPage > 1;

        public static bool HasNext(int currentPage, int totalPages) => currentPage < totalPages;
    }
}
