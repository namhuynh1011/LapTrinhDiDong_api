using System.ComponentModel.DataAnnotations;

namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class CreateAppointmentRequest
  {
    [Required, MaxLength(200)]
    public string PatientFullName { get; set; }
    [Required]
    public Guid PatientId { get; set; }
    [Required]
    public Guid DoctorId { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    [MaxLength(500)]
    public string? PatientAddress { get; set; }
    [Required]
    public DateTime AppointmentDate { get; set; }
    [Required, MaxLength(5)]
    public string AppointmentTime { get; set; }
    [MaxLength(2000)]
    public string? Note { get; set; }
  }
}