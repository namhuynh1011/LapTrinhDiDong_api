namespace LapTrinhDiDong_api.Models.ModelsRequest
{
    public class UpdateAppointmentRequest
    {
        public Guid? DoctorId { get; set; }
        public Guid? PatientId { get; set; }

        // Optional fields to update
        public string? PatientFullName { get; set; }
        public string? Phone { get; set; }
        public string? PatientAddress { get; set; }

        // Date/time optional (date: yyyy-MM-dd, time: HH:mm)
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }

        public string? Note { get; set; }
        public string? Status { get; set; }
        public string? CancelReason { get; set; }
    }
}