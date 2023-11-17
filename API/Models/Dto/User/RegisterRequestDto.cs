namespace Product_API.Models.Dto
{
  public class RegisterRequestDto
  {
    public string UserName { get; set; }
    public string Names { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
  }
}