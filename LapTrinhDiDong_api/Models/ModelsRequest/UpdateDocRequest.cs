namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class UpdateDoctorRequest
  {
    public string? FullName { get; set; }
    public string? Hospital { get; set; }

    // Prefer id if provided
    public Guid? SpecialtyId { get; set; }

    // Or accept name (frontend may send name)
    public string? SpecialtyName { get; set; }

    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
  }
}