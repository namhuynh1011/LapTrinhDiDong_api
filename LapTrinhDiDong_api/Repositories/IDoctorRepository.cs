using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Models.ModelsRequest;

namespace LapTrinhDiDong_api.Repositories
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
        Task<Doctor?> GetDoctorByIdAsync(Guid id);
        Task<Doctor> CreateDoctorAsync(CreateDocRequest dto);
        Task<Doctor?> UpdateDoctorAsync(Guid id, UpdateDoctorRequest doctor, bool createSpecialtyIfMissing = false);
        Task<bool> DeleteDoctorAsync(Guid id);
    }
}