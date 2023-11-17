using System.ComponentModel.DataAnnotations;

namespace ProductDto_API.Models.KindOfProduct
{
  public class KindOfProductDto
  {
    public int KindOfProductId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    [Required]
    public string Detail { get; set; }
  }
}