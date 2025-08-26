namespace TodoListAPI.Models;

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<T> Items { get; set; }

    public PagedResult(List<T> source, int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
        Total = source.Count;
        Items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
}