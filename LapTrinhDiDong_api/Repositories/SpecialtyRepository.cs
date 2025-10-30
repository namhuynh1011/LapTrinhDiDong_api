using LapTrinhDiDong_api.Data;
using LapTrinhDiDong_api.Models;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhDiDong_api.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly ApplicationDbContext _context;

        public SpecialtyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
        {
            return await _context.Specialties.ToListAsync();
        }

        public async Task<Specialty> GetSpecialtyByIdAsync(Guid id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<Specialty> CreateSpecialtyAsync(Specialty specialty)
        {
            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<Specialty> UpdateSpecialtyAsync(Guid id, Specialty specialty)
        {
            var existingSpecialty = await _context.Specialties.FindAsync(id);
            if (existingSpecialty == null)
            {
                return null;
            }

            existingSpecialty.Name = specialty.Name;
            await _context.SaveChangesAsync();
            return existingSpecialty;
        }

        public async Task<bool> DeleteSpecialtyAsync(Guid id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return false;
            }

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}