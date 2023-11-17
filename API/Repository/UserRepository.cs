using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<UserApplication> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserRepository(
      AppDbContext db,
      IConfiguration configuration,
      UserManager<UserApplication> userManager,
      RoleManager<IdentityRole> roleManager,
      IMapper mapper
    )
    {
      _db = db;
      secretKey = configuration.GetValue<string>("ApiSettings:Secret");
      _userManager = userManager;
      _roleManager = roleManager;
      _mapper = mapper;
    }
    public bool IsUniqueUser(string userName)
    {
      var user = _db.UsersApplication.FirstOrDefault(user => user.UserName.ToLower() == userName.ToLower());
      if (user == null)
      {
        return true;
      }
      return false;
    }
    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
      var user = await _db.UsersApplication.FirstOrDefaultAsync(
        user => user.UserName.ToLower() == loginRequestDto.UserName.ToLower());

      bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

      if (user == null || isValid == false)
      {
        return new LoginResponseDto()
        {
          Token = "",
          User = null
        };
      }
      // Generate JWT
      var roles = await _userManager.GetRolesAsync(user);
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(secretKey);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
          new Claim(ClaimTypes.Name, user.Id.ToString()),
          new Claim(ClaimTypes.Role, roles.FirstOrDefault())
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      LoginResponseDto loginResponseDto = new()
      {
        Token = tokenHandler.WriteToken(token),
        User = _mapper.Map<UserDto>(user),
        Role = roles.FirstOrDefault()
      };
      return loginResponseDto;
    }
    public async Task<UserDto> Register(RegisterRequestDto registerRequestDto)
    {
      UserApplication user = new()
      {
        UserName = registerRequestDto.UserName,
        Email = registerRequestDto.UserName,
        NormalizedEmail = registerRequestDto.UserName.ToUpper(),
        Names = registerRequestDto.Names,
      };

      try
      {
        var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
        Console.WriteLine(result);
        if (result.Succeeded)
        {
          if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
          {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("client"));
          }
          await _userManager.AddToRoleAsync(user, "admin");
          var userApp = _db.UsersApplication.FirstOrDefault(u => u.UserName == registerRequestDto.UserName);
          return _mapper.Map<UserDto>(userApp);
        }
      }
      catch (Exception)
      {
        throw;
      }
      return new UserDto();
    }
  }
}