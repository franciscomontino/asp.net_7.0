using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.Product
{
  public class KindOfProductCreateDto
  {
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    [Required]
    public string Detail { get; set; }
  }
}