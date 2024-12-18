using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.CustomModels
{
    public class PaginatedResponse<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; } = false;
        public bool HasNext { get; set; } = false;
        public IEnumerable<T> Data { get; set; }

        public PaginatedResponse(IEnumerable<T> currentItems, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            Data = currentItems;

            if (pageNumber > 1)
            {
                HasPrevious = true;
            }

            if (pageNumber < TotalPages)
            {
                HasNext = true;
            }

        }

        public static async Task<PaginatedResponse<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {

            var count = await source.CountAsync();

            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            pageNumber = pageNumber > totalPages ? totalPages : pageNumber;
            
            pageNumber = pageNumber < 1 ? 1 : pageNumber;


            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedResponse<T>(items, count, pageNumber, pageSize);
        }

    }

    //public class PagedList<T> : List<T>
    //{
    //    public int CurrentPage { get; set; }
    //    public int TotalPages { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalCount { get; set; }
    //    public bool HasPrevious { get; set; } = false;
    //    public bool HasNext { get; set; } = false;

    //    public PagedList(IEnumerable<T> currentItems, int count, int pageNumber, int pageSize)
    //    {
    //        CurrentPage = pageNumber;
    //        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    //        PageSize = pageSize;
    //        TotalCount = count;

    //        if (pageNumber > 1)
    //        {
    //            HasPrevious = true;
    //        }

    //        if (pageNumber < TotalPages)
    //        {
    //            HasNext = true;
    //        }

    //        AddRange(currentItems);
    //    }

    //    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    //    {
    //        pageNumber = pageNumber < 1 ? 1 : pageNumber;
    //        var count = await source.CountAsync();
    //        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    //        return new PagedList<T>(items, count, pageNumber, pageSize);
    //    }

    //}
}
