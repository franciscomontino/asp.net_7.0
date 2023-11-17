using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.Product
{
  public class ProductDto
  {
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    public string Detail { get; set; }

    [Required]
    public double Price { get; set; }

    public string ImageUrl { get; set; }

    [Required]
    public double KindOfProductId { get; set; }
  }
}