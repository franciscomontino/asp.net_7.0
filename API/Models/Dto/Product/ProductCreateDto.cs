using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.Product
{
  public class ProductCreateDto
  {
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    [Required]
    public string Detail { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public string ImageUrl { get; set; }

    [Required]
    public double KindOfProductId { get; set; }
  }
}