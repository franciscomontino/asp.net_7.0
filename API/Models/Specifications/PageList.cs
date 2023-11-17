namespace Product_API.Models.Specifications
{
  public class PageList<T> : List<T>
  {
    public MetaData MetaData { get; set; }

    public PageList(List<T> items, int count, int pageNumber, int pageSize)
    {
      MetaData = new MetaData
      {
        TotalCount = count,
        PageSize = pageSize,
        TotalPage = (int)Math.Ceiling(count / (double)pageSize)
      };
      AddRange(items);
    }

    public static PageList<T> ToPagedList(IEnumerable<T> entity, int pageNumber, int pageSize)
    {
      var count = entity.Count();
      var items = entity
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();
      return new PageList<T>(items, count, pageNumber, pageSize);
    }
  }
}