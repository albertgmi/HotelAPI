namespace HotelAPI.Models
{
    public class PagedResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalPages { get; set; }
        public int ItemFrom { get; set; }
        public int ItemTo { get; set; }
        public int TotalItemsCount { get; set; }
        public PagedResult(List<T> results, int totalItemsCount, int pageSize, int pageNumber)
        {
            Results = results;
            TotalItemsCount = totalItemsCount;
            ItemFrom = pageSize * (pageNumber - 1) + 1;
            ItemTo = ItemFrom + pageSize - 1;
            TotalPages = (int)Math.Ceiling(totalItemsCount / (double)pageSize);
        }
    }
}
