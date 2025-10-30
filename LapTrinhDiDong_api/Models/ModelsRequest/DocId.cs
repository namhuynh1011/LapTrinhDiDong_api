namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class DocId
  {
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? Hospital { get; set; }

    public string? SpecialtyName { get; set; }

    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
  }
}