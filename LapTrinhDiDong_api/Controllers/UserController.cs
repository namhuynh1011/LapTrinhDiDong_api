using LapTrinhDiDong_api.Repositories;
using LapTrinhDiDong_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LapTrinhDiDong_api.Models.ModelsRequest;
using System.Security.Claims;

namespace LapTrinhDiDong_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && currentUserId != id.ToString())
            {
                return Forbid(); // Trả về 403 Forbidden
            }
            if (id == Guid.Empty)
            {
                return BadRequest("ID không hợp lệ");
            }
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Phone,
                user.Address,
                user.BirthDay,
                user.Gender,
                user.Role
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role
            }).ToList();

            return Ok(dtos);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("ID không hợp lệ");
            }
            await _userRepository.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPut("update-user/{id}")]
        public async Task<ActionResult<User>> UpdateUser(Guid id, [FromBody] UpdateUserRequest user)
        {
            if (user == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }


            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound("User không tồn tại");
            }

            existingUser.FullName = user.FullName;
            existingUser.Phone = user.Phone;
            existingUser.Address = user.Address;
            existingUser.BirthDay = user.BirthDay;
            existingUser.Gender = user.Gender;
            existingUser.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.UpdateUserAsync(existingUser);
            return Ok(new
            {
                existingUser.Id,
                existingUser.FullName,
                existingUser.Email,
                existingUser.Phone,
                existingUser.Address,
                existingUser.BirthDay,
            });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            // 1. Kiểm tra validation của model trước
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Lấy thông tin người dùng từ token (claims)
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {

                return Unauthorized();
            }


            var user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {

                return NotFound(new { message = "Tài khoản không tồn tại!" });
            }

            // 3. Kiểm tra mật khẩu cũ không được trùng mật khẩu mới
            if (model.OldPassword == model.NewPassword)
            {
                return BadRequest(new { message = "Mật khẩu mới không được trùng với mật khẩu cũ." });
            }

            // Xác thực mật khẩu cũ
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password);
            if (!isOldPasswordValid)
            {
                return BadRequest(new { message = "Mật khẩu cũ không đúng!" });
            }

            // Hash và cập nhật mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            try
            {
                await _userRepository.UpdateUserAsync(user);
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau." });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var userIdCaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdCaim == null)
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdCaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Phone,
                user.Address,
                user.BirthDay,

            });
        }
    }
}