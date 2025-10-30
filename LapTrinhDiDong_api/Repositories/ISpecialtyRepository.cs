using LapTrinhDiDong_api.Models;

namespace LapTrinhDiDong_api.Repositories
{
    public interface ISpecialtyRepository
    {
        Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync();
        Task<Specialty?> GetSpecialtyByIdAsync(Guid id);
        Task<Specialty> CreateSpecialtyAsync(Specialty specialty);
        Task<Specialty?> UpdateSpecialtyAsync(Guid id, Specialty specialty);
        Task<bool> DeleteSpecialtyAsync(Guid id);
    }
}