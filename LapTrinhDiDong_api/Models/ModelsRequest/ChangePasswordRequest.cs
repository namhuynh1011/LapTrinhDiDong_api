using System.ComponentModel.DataAnnotations;

namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class ChangePasswordRequest
  {
    [Required]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.")]
    public string NewPassword { get; set; }
  }
}