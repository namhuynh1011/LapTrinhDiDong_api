namespace LapTrinhDiDong_api.Models.ModelsRequest
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime BirthDay { get; set; }
        public string Gender { get; set; }
        public Role Role { get; set; }
    }
}