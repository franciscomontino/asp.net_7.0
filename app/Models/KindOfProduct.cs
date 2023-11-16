using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product_API.Models
{
  public class KindOfProduct
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int KindOfProductId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Detail { get; set; }

    public DateTime Create { get; set; }

    public DateTime Update { get; set; }
  }
}