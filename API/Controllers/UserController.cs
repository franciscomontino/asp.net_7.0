using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Product_API.Models;
using Product_API.Models.Dto;
using Product_API.Repository.IRepository;

namespace Product_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IUserRepository _userRepository;
    private ApiResponse _response;

    public UserController(IUserRepository userRepository)
    {
      _userRepository = userRepository;
      _response = new();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
      var loginResponse = await _userRepository.Login(model);
      if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
      {
        _response.statusCode = HttpStatusCode.BadRequest;
        _response.isSuccessful = false;
        _response.ErrorMessages.Add("Wrong username or password");
        return BadRequest(_response);
      }
      _response.statusCode = HttpStatusCode.OK;
      _response.Result = loginResponse;
      return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
    {
      bool isUniqueUser = _userRepository.IsUniqueUser(model.UserName);
      if (!isUniqueUser)
      {
        _response.statusCode = HttpStatusCode.BadRequest;
        _response.isSuccessful = false;
        _response.ErrorMessages.Add("User exists");
        return BadRequest(_response);
      }
      var user = await _userRepository.Register(model);
      if (user == null)
      {
        _response.statusCode = HttpStatusCode.BadRequest;
        _response.isSuccessful = false;
        _response.ErrorMessages.Add("error registering user");
        return BadRequest(_response);
      }
      _response.statusCode = HttpStatusCode.OK;
      return Ok(_response);
    }
  }
}
