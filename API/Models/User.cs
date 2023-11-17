using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product_API.Models
{
  public class User
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
  }
}