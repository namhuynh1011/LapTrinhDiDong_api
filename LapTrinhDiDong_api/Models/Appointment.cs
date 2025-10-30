using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LapTrinhDiDong_api.Models
{
  public enum Status
  {
    Pending,
    Confirmed,
    Completed,
    Cancelled
  }
  public class Appointment
  {
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string PatientFullName { get; set; }
    [Required]
    public Guid PatientId { get; set; }
    [ForeignKey(nameof(PatientId))]
    [JsonIgnore]
    public User? User { get; set; }

    [Required]
    public Guid DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    [JsonIgnore]
    public Doctor? Doctor { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    [MaxLength(500)]
    public string? PatientAddress { get; set; }

    [Column(TypeName = "date")]
    public DateTime AppointmentDate { get; set; }
    [Column(TypeName = "time")]
    public TimeSpan AppointmentTime { get; set; }
    [MaxLength(2000)]
    public string? Note { get; set; }
    [Required]
    public Status Status { get; set; }
    [MaxLength(1000)]
    public string? CancelReason { get; set; }
  }
}