namespace LapTrinhDiDong_api.Models.ModelsRequest
{
  public class AppointmentResponse
  {
    public Guid Id { get; set; }
    public string PatientFullName { get; set; } = "";
    public Guid PatientId { get; set; }

    // MỚI: trả về doctor id + (tuỳ chọn) tên bác sĩ
    public Guid DoctorId { get; set; }
    public string? DoctorName { get; set; }

    public string? Phone { get; set; }
    public string? PatientAddress { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime { get; set; }// "HH:mm"
    public string? Note { get; set; }
    public string Status { get; set; }
    public string? CancelReason { get; set; }
  }
}