using LapTrinhDiDong_api.Data;
using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Models.ModelsRequest;
using LapTrinhDiDong_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LapTrinhDiDong_api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AppointmentController : ControllerBase
  {
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ApplicationDbContext _context;
    public AppointmentController(IAppointmentRepository appointmentRepository, ApplicationDbContext context)
    {
      _appointmentRepository = appointmentRepository;
      _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest dto)
    {
      // Validate Doctor exist
      var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
      if (doctor == null)
        return BadRequest($"Doctor with id {dto.DoctorId} not found.");

      // Validate Patient exist (optional)
      var patient = await _context.Users.FindAsync(dto.PatientId);
      if (patient == null)
        return BadRequest($"Patient with id {dto.PatientId} not found.");

      // parse time
      if (!TimeSpan.TryParseExact(dto.AppointmentTime, @"hh\:mm", null, out var timeSpan) &&
          !TimeSpan.TryParseExact(dto.AppointmentTime, @"h\:mm", null, out timeSpan))
      {
        return BadRequest("Invalid AppointmentTime format. Expected 'HH:mm'.");
      }

      // check appointment date not in past (optional)
      if (dto.AppointmentDate.Date < DateTime.UtcNow.Date)
        return BadRequest("AppointmentDate cannot be in the past.");

      var appt = new Appointment
      {
        PatientFullName = dto.PatientFullName,
        PatientId = dto.PatientId,
        DoctorId = dto.DoctorId,
        Phone = dto.Phone,
        PatientAddress = dto.PatientAddress,
        AppointmentDate = dto.AppointmentDate.Date,
        AppointmentTime = timeSpan,
        Note = dto.Note,
        Status = Status.Pending,
      };

      _context.Appointments.Add(appt);
      await _context.SaveChangesAsync();

      // map to DTO to return
      var result = new AppointmentResponse
      {
        Id = appt.Id,
        PatientFullName = appt.PatientFullName,
        PatientId = appt.PatientId,
        DoctorId = appt.DoctorId,
        DoctorName = doctor?.FullName, // set if available
        Phone = appt.Phone,
        PatientAddress = appt.PatientAddress,
        AppointmentDate = appt.AppointmentDate,
        AppointmentTime = appt.AppointmentTime.ToString(@"hh\:mm"),
        Note = appt.Note,
        Status = appt.Status.ToString(),
      };

      return CreatedAtAction(nameof(GetById), new { id = appt.Id }, result);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
      var appt = await _context.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id);
      if (appt == null) return NotFound();

      var dto = new AppointmentResponse
      {
        Id = appt.Id,
        PatientFullName = appt.PatientFullName,
        PatientId = appt.PatientId,
        DoctorId = appt.DoctorId,
        DoctorName = appt.Doctor?.FullName,
        Phone = appt.Phone,
        PatientAddress = appt.PatientAddress,
        AppointmentDate = appt.AppointmentDate,
        AppointmentTime = appt.AppointmentTime.ToString(@"hh\:mm"),
        Note = appt.Note,
        Status = appt.Status.ToString(),
        CancelReason = appt.CancelReason,
      };
      return Ok(dto);
    }
    [HttpGet]
    public async Task<IActionResult> List(
            [FromQuery] Guid? patientId,
            [FromQuery] Guid? doctorId,
            [FromQuery] string? status,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
    {
      if (page <= 0) page = 1;
      if (pageSize <= 0 || pageSize > 200) pageSize = 20;

      IQueryable<Appointment> query = _context.Appointments.Include(a => a.Doctor);

      if (patientId.HasValue)
        query = query.Where(a => a.PatientId == patientId.Value);

      if (doctorId.HasValue)
        query = query.Where(a => a.DoctorId == doctorId.Value);

      if (!string.IsNullOrWhiteSpace(status))
      {
        if (Enum.TryParse<Status>(status, true, out var st))
        {
          query = query.Where(a => a.Status == st);
        }
        else
        {
          // ignore unknown status filter
        }
      }

      if (dateFrom.HasValue)
        query = query.Where(a => a.AppointmentDate >= dateFrom.Value.Date);

      if (dateTo.HasValue)
        query = query.Where(a => a.AppointmentDate <= dateTo.Value.Date);

      var total = await query.CountAsync();
      var items = await query
          .OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      var results = items.Select(a => new AppointmentResponse
      {
        Id = a.Id,
        PatientFullName = a.PatientFullName,
        PatientId = a.PatientId,
        DoctorId = a.DoctorId,
        DoctorName = a.Doctor?.FullName,
        Phone = a.Phone,
        PatientAddress = a.PatientAddress,
        AppointmentDate = a.AppointmentDate,
        AppointmentTime = a.AppointmentTime.ToString(@"hh\:mm"),
        Note = a.Note,
        Status = a.Status.ToString(),
        CancelReason = a.CancelReason,
      }).ToList();

      return Ok(new
      {
        total,
        page,
        pageSize,
        data = results
      });
    }

    // Convenience: list by patient (paginated)
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetByPatient(Guid patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
      return await Task.FromResult(RedirectToAction(nameof(List), new { patientId = patientId, page = page, pageSize = pageSize }));
    }

    // Convenience: list by doctor (paginated)
    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
      return await Task.FromResult(RedirectToAction(nameof(List), new { doctorId = doctorId, page = page, pageSize = pageSize }));
    }

    // UPDATE appointment details (PUT)
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentRequest dto)
    {
      var appt = await _context.Appointments.FindAsync(id);
      if (appt == null) return NotFound();

      // Nếu cập nhật doctorId -> kiểm tra tồn tại
      if (dto.DoctorId.HasValue)
      {
        var doc = await _context.Doctors.FindAsync(dto.DoctorId.Value);
        if (doc == null) return BadRequest($"Doctor with id {dto.DoctorId} not found.");
        appt.DoctorId = dto.DoctorId.Value;
      }

      // Nếu cập nhật patientId -> kiểm tra tồn tại
      if (dto.PatientId.HasValue)
      {
        var pat = await _context.Users.FindAsync(dto.PatientId.Value);
        if (pat == null) return BadRequest($"Patient with id {dto.PatientId} not found.");
        appt.PatientId = dto.PatientId.Value;
      }

      if (!string.IsNullOrWhiteSpace(dto.PatientFullName)) appt.PatientFullName = dto.PatientFullName;
      if (!string.IsNullOrWhiteSpace(dto.Phone)) appt.Phone = dto.Phone;
      if (!string.IsNullOrWhiteSpace(dto.PatientAddress)) appt.PatientAddress = dto.PatientAddress;
      if (!string.IsNullOrWhiteSpace(dto.Note)) appt.Note = dto.Note;

      if (dto.AppointmentDate.HasValue)
      {
        if (dto.AppointmentDate.Value.Date < DateTime.UtcNow.Date)
          return BadRequest("AppointmentDate cannot be in the past.");
        appt.AppointmentDate = dto.AppointmentDate.Value.Date;
      }

      if (!string.IsNullOrWhiteSpace(dto.AppointmentTime))
      {
        if (!TimeSpan.TryParseExact(dto.AppointmentTime, @"hh\:mm", null, out var ts) &&
            !TimeSpan.TryParseExact(dto.AppointmentTime, @"h\:mm", null, out ts))
        {
          return BadRequest("Invalid AppointmentTime format. Expected 'HH:mm'.");
        }
        appt.AppointmentTime = ts;
      }

      // Nếu client gửi Status -> xử lý thay đổi trạng thái tại đây
      if (!string.IsNullOrWhiteSpace(dto.Status))
      {
        if (!Enum.TryParse<Status>(dto.Status, true, out var newStatus))
          return BadRequest("Invalid status value.");

        if (newStatus == Status.Cancelled)
        {
          if (string.IsNullOrWhiteSpace(dto.CancelReason))
            return BadRequest("CancelReason is required when cancelling an appointment.");
          appt.CancelReason = dto.CancelReason;
        }
        else
        {
          // Khi chuyển trạng thái khác, xoá cancel reason nếu có
          appt.CancelReason = null;
        }

        appt.Status = newStatus;
      }


      _context.Appointments.Update(appt);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    // DELETE - soft-cancel by default; hard delete when ?force=true
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool force = false)
    {
      var appt = await _context.Appointments.FindAsync(id);
      if (appt == null) return NotFound();

      if (force)
      {
        _context.Appointments.Remove(appt);
        await _context.SaveChangesAsync();
        return NoContent();
      }

      // soft cancel
      appt.Status = Status.Cancelled;
      appt.CancelReason = "Bệnh nhân huỷ cuộc hẹn.";
      _context.Appointments.Update(appt);
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}