namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class UpdateUserRequest
  {
    public string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDay { get; set; }
    public string? Gender { get; set; }
  }
}