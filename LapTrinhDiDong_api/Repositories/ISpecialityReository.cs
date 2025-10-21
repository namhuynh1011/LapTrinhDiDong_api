using LapTrinhDiDong_api.Models;

namespace LapTrinhDiDong_api.Repositories
{
    public interface ISpecialityReository
    {
        Task<IEnumerable<Speciality>> GetAllSpecialitiesAsync();
        Task<Speciality?> GetSpecialityByIdAsync(Guid id);
        Task<Speciality> CreateSpecialityAsync(Speciality speciality);
        Task<Speciality?> UpdateSpecialityAsync(Guid id, Speciality speciality);
        Task<bool> DeleteSpecialityAsync(Guid id);
    }
}