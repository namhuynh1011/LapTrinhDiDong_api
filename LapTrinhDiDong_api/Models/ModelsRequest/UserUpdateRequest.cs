namespace LapTrinhDiDong_api.Models.ModelsRequest
{
    public class UserUpdateRequest
    {
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int? Birthyear { get; set; }

        public string? Gender { get; set; }
    }
}