namespace PGAdmin.Models;

public class PagedData<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
}