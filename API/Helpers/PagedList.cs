using Microsoft.EntityFrameworkCore;

namespace API;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumer, int pageSize)
    {
        this.CurrentPage = pageNumer;
        this.TotalPages = (int)Math.Ceiling(count / (double)pageSize); //wont always divide easily so use upperbound for num pages
        this.PageSize = pageSize;
        this.TotalCount = count;
        AddRange(items);
    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,
    int pageNumber, int pageSize)
    {
        var count = await source.CountAsync(); //counts num of elements requtured from the query.

        var items = await source.Skip((pageNumber - 1) * pageSize) //The result of step 1 is multiplied by the page size to determine the total number of elements to skip. By multiplying the adjusted page number by the page size, we can calculate the appropriate offset.For example, let's consider a scenario where the page number is 3 and the page size is 10. Using the formula (pageNumber - 1) * pageSize , we can calculate:(3 - 1) * 10 = 20

        .Take(pageSize)//takes a number of elements specified and adds them to the list
        .ToListAsync(); //creates the list based off the query executing in the databse. in this case with the num of elements per page and the page we are on

        return new PagedList<T>(items, count, pageNumber, pageSize); //returns an instance of a list for that page from the database
    }
}
