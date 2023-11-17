using Product_API.Models;
using Product_API.Models.Dto;

namespace Product_API.Repository.IRepository
{
  public interface IUserRepository
  {
    bool IsUniqueUser(string userName);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    Task<UserDto> Register(RegisterRequestDto registerRequestDto);
  }
}