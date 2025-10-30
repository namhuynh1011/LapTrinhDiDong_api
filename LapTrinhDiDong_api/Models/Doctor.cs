using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LapTrinhDiDong_api.Models
{
  public class Doctor
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; }
    [MaxLength(250)]
    public string? Hospital { get; set; }
    [Required]
    public Guid SpecialtyId { get; set; }
    [ForeignKey(nameof(SpecialtyId))]
    [JsonIgnore]
    public Specialty? Specialty { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }

    // URL tới ảnh đại diện
    [MaxLength(1000)]
    public string? AvatarUrl { get; set; }
  }
}