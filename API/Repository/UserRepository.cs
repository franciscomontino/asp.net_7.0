using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product_API.Data;
using Product_API.Models;
using Product_API.Models.Dto;
using Product_API.Repository.IRepository;

namespace Product_API.Repository
{
  public class UserRepository : IUserRepository
  {
    private readonly AppDbContext _db;
    private string secretKey;

    public UserRepository(AppDbContext db, IConfiguration configuration)
    {
      _db = db;
      secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }
    public bool IsUniqueUser(string userName)
    {
      var user = _db.Users.FirstOrDefault(user => user.UserName.ToLower() == userName.ToLower());
      if (user == null)
      {
        return true;
      }
      return false;
    }
    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
      var user = await _db.Users.FirstOrDefaultAsync(
        user => user.UserName.ToLower() == loginRequestDto.UserName.ToLower()
        && user.Password == loginRequestDto.Password
      );
      if (user == null)
      {
        return new LoginResponseDto()
        {
          Token = "",
          User = null
        };
      }
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(secretKey);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
          new Claim(ClaimTypes.Name, user.Id.ToString()),
          new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      LoginResponseDto loginResponseDto = new()
      {
        Token = tokenHandler.WriteToken(token),
        User = user
      };
      return loginResponseDto;
    }
    public async Task<User> Register(RegisterRequestDto registerRequestDto)
    {
      User user = new()
      {
        UserName = registerRequestDto.UserName,
        Password = registerRequestDto.Password,
        Name = registerRequestDto.Names,
        Role = registerRequestDto.Role,
      };
      await _db.Users.AddAsync(user);
      await _db.SaveChangesAsync();
      user.Password = "";
      return user;
    }
  }
}