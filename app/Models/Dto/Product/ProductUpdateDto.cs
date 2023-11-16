using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.Product
{
  public class ProductUpdateDto
  {
    public int Id { get; set; }

    [MaxLength(30)]
    public string Name { get; set; }

    public string Detail { get; set; }

    public double Price { get; set; }

    public string ImageUrl { get; set; }

    public double KindOfProductId { get; set; }
  }
}