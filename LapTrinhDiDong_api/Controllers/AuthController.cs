using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BCrypt.Net;
using LapTrinhDiDong_api.Repositories;
using LapTrinhDiDong_api.Models.ModelsRequest;
using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace LapTrinhDiDong_api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;
    public AuthController(IUserRepository userRepository, IConfiguration config)
    {
      _userRepository = userRepository;
      _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
      // Kiểm tra email đã tồn tại chưa
      var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
      if (existingUser != null)
      {
        return BadRequest(new { message = "Email đã tồn tại!" });
      }

      // Hash mật khẩu
      string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

      // Tạo user mới
      var user = new User
      {
        Id = Guid.NewGuid(),
        FullName = model.FullName,
        Email = model.Email,
        Password = passwordHash,
        Role = Role.Patient // mặc định cho bệnh nhân
      };

      await _userRepository.AddUserAsync(user);
      return Ok(new { message = "Đăng ký thành công!" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
      var user = await _userRepository.GetUserByEmailAsync(model.Email);
      if (user == null)
        return Unauthorized(new { message = "Tài khoản không tồn tại!" });

      bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
      if (!isPasswordValid)
        return Unauthorized(new { message = "Tài Khoản Không Tồn Tại Hoặc Sai mật khẩu!" });

      // Sinh token
      var token = JwtHelper.GenerateJwtToken(user, _config);

      return Ok(new
      {
        token,
        user = new
        {
          id = user.Id,
          fullName = user.FullName,
          email = user.Email,
          phone = user.Phone,
          role = user.Role
        }
      });
    }
  }
}