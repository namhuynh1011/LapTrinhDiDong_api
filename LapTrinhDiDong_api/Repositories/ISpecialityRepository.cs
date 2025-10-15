using LapTrinhDiDong_api.Models;

namespace LapTrinhDiDong_api.Repositories
{
    public interface ISpecialityRepository
    {
        Task<IEnumerable<Speciality>> GetAllSpecialitiesAsync();
        Task<Speciality> GetSpecialityByIdAsync(Guid id);
        Task<Speciality> CreateSpecialityAsync(Speciality speciality);
        Task<Speciality> UpdateSpecialityAsync(Speciality speciality);
        Task<bool> DeleteSpecialityAsync(Guid id);
    }
}