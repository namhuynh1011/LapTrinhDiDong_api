using LapTrinhDiDong_api.Data;
using LapTrinhDiDong_api.Models;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhDiDong_api.Repositories
{
    public class SpecialityRepository : ISpecialityRepository
    {
        private readonly ApplicationDbContext _context;

        public SpecialityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Speciality>> GetAllSpecialitiesAsync()
        {
            return await _context.Specialities.ToListAsync();
        }

        public async Task<Speciality> GetSpecialityByIdAsync(Guid id)
        {
            return await _context.Specialities.FindAsync(id);
        }

        public async Task<Speciality> CreateSpecialityAsync(Speciality speciality)
        {
            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();
            return speciality;
        }

        public async Task<Speciality> UpdateSpecialityAsync(Speciality speciality)
        {
            var existingSpeciality = await _context.Specialities.FindAsync(speciality.Id);
            if (existingSpeciality == null)
            {
                return null;
            }

            existingSpeciality.Name = speciality.Name;
            await _context.SaveChangesAsync();
            return existingSpeciality;
        }

        public async Task<bool> DeleteSpecialityAsync(Guid id)
        {
            var speciality = await _context.Specialities.FindAsync(id);
            if (speciality == null)
            {
                return false;
            }

            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}