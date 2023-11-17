using Microsoft.AspNetCore.Identity;

namespace Product_API.Models
{
  public class UserApplication : IdentityUser
  {
    public string Names { get; set; }
  }
}