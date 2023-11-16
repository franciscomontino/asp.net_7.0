using System.Net;

namespace Product_API.Models
{
  public class ApiResponse
  {
    public HttpStatusCode statusCode { get; set; }
    public bool isSuccessful { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }
  }
}