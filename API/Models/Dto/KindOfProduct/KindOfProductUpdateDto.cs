using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.KindOfProduct
{
  public class KindOfProductUpdateDto
  {
    public int KindOfProductId { get; set; }

    public string Name { get; set; }

    public string Detail { get; set; }
  }
}