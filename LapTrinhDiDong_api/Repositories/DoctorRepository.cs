using LapTrinhDiDong_api.Data;
using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Models.ModelsRequest;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhDiDong_api.Repositories
{
  public class DoctorRepository : IDoctorRepository
  {
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
      return await _context.Doctors.ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
    {
      return await _context.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Doctor> CreateDoctorAsync(CreateDocRequest dto)
    {
      if (string.IsNullOrWhiteSpace(dto.FullName))
        throw new ArgumentException("FullName is required.", nameof(dto.FullName));

      // Resolve specialty: prefer SpecialtyId if provided, otherwise use SpecialtyName
      Specialty? specialty = null;
      if (!string.IsNullOrWhiteSpace(dto.SpecialtyName))
      {
        var nameNormalized = dto.SpecialtyName.Trim().ToLowerInvariant();
        specialty = await _context.Specialties
            .FirstOrDefaultAsync(s => s.Name.ToLower() == nameNormalized);
        if (specialty == null)
        {
          // Option A: throw error (safer)
          throw new ArgumentException($"Specialty with name '{dto.SpecialtyName}' does not exist.", nameof(dto.SpecialtyName));

          // Option B (alternative): create new Specialty automatically
          // specialty = new Specialty { Id = Guid.NewGuid(), Name = dto.SpecialtyName.Trim() };
          // _context.Specialties.Add(specialty);
          // await _context.SaveChangesAsync();
        }
      }
      else
      {
        throw new ArgumentException("Either SpecialtyId or SpecialtyName must be provided.");
      }

      var doctor = new Doctor
      {
        Id = Guid.NewGuid(),
        FullName = dto.FullName.Trim(),
        Hospital = string.IsNullOrWhiteSpace(dto.Hospital) ? null : dto.Hospital.Trim(),
        SpecialtyId = specialty.Id,
        Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
        AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? null : dto.AvatarUrl.Trim(),
      };

      _context.Doctors.Add(doctor);
      await _context.SaveChangesAsync();

      // Eager-load specialty for return
      doctor = await _context.Doctors.Include(d => d.Specialty).FirstAsync(d => d.Id == doctor.Id);

      return doctor;
    }

    public async Task<Doctor> UpdateDoctorAsync(Guid id, UpdateDoctorRequest dto, bool createSpecialtyIfMissing = false)
    {
      var doctor = await _context.Doctors.FindAsync(id);
      if (doctor == null) throw new ArgumentException($"Doctor with id {id} not found.", nameof(id));

      // Resolve specialty
      Specialty? specialty = null;
      if (dto.SpecialtyId.HasValue && dto.SpecialtyId != Guid.Empty)
      {
        specialty = await _context.Specialties.FindAsync(dto.SpecialtyId.Value);
        if (specialty == null)
          throw new ArgumentException($"Specialty with id {dto.SpecialtyId} does not exist.", nameof(dto.SpecialtyId));
      }
      else if (!string.IsNullOrWhiteSpace(dto.SpecialtyName))
      {
        var nameNormalized = dto.SpecialtyName.Trim();
        specialty = await _context.Specialties.FirstOrDefaultAsync(s => s.Name == nameNormalized);
        if (specialty == null)
        {
          if (createSpecialtyIfMissing)
          {
            specialty = new Specialty { Id = Guid.NewGuid(), Name = nameNormalized };
            _context.Specialties.Add(specialty);
            // don't SaveChanges here yet; will save together below
          }
          else
          {
            throw new ArgumentException($"Specialty with name '{dto.SpecialtyName}' does not exist.", nameof(dto.SpecialtyName));
          }
        }
      }
      else
      {
        // if neither provided, keep existing specialty (or you can choose to clear it)
        specialty = null;
      }

      // Apply updates
      if (!string.IsNullOrWhiteSpace(dto.FullName)) doctor.FullName = dto.FullName.Trim();
      doctor.Hospital = string.IsNullOrWhiteSpace(dto.Hospital) ? null : dto.Hospital.Trim();
      doctor.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
      doctor.AvatarUrl = string.IsNullOrWhiteSpace(dto.AvatarUrl) ? null : dto.AvatarUrl.Trim();

      if (specialty != null)
      {
        doctor.SpecialtyId = specialty.Id;
      }

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateException dbEx)
      {
        // Log dbEx in real app
        throw new InvalidOperationException("Database update failed when updating doctor. " + dbEx.Message, dbEx);
      }

      // reload with navigation
      return await GetDoctorByIdAsync(doctor.Id) ?? doctor;
    }

    public async Task<bool> DeleteDoctorAsync(Guid id)
    {
      var doctor = await _context.Doctors.FindAsync(id);
      if (doctor == null)
      {
        return false;
      }

      _context.Doctors.Remove(doctor);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}