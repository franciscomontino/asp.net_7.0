using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product_API.Models
{
  public class Product
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Detail { get; set; }

    [Required]
    public double Price { get; set; }

    public string ImageUrl { get; set; }

    public DateTime Create { get; set; }

    public DateTime Update { get; set; }
  }
}