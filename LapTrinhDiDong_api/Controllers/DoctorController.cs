using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using LapTrinhDiDong_api.Models.ModelsRequest;
namespace LapTrinhDiDong_api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class DoctorController : ControllerBase
  {
    private readonly IDoctorRepository _doctorRepository;

    public DoctorController(IDoctorRepository doctorRepository)
    {
      _doctorRepository = doctorRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Doctor>>> GetAllDoctors()
    {
      var doctors = await _doctorRepository.GetAllDoctorsAsync();
      return Ok(doctors);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocId>> GetDoctorById(Guid id)
    {
      var doc = await _doctorRepository.GetDoctorByIdAsync(id);
      if (doc == null) return NotFound();

      var dto = new DocId
      {
        Id = doc.Id,
        FullName = doc.FullName,
        Hospital = doc.Hospital,
        SpecialtyName = doc.Specialty?.Name,
        Phone = doc.Phone,
        AvatarUrl = doc.AvatarUrl
      };

      return Ok(dto);
    }


    [HttpPost]
    public async Task<ActionResult<DocId>> CreateDoctor([FromBody] CreateDocRequest request)
    {
      if (request == null) return BadRequest("Request body is null.");
      if (string.IsNullOrWhiteSpace(request.FullName))
      {
        ModelState.AddModelError(nameof(request.FullName), "FullName is required.");
        return ValidationProblem(ModelState);
      }

      try
      {
        var created = await _doctorRepository.CreateDoctorAsync(request);

        var dto = new DocId
        {
          Id = created.Id,
          FullName = created.FullName,
          Hospital = created.Hospital,
          SpecialtyName = created.Specialty?.Name,
          Phone = created.Phone,
          AvatarUrl = created.AvatarUrl
        };

        return CreatedAtAction(nameof(GetDoctorById), new { id = dto.Id }, dto);
      }
      catch (ArgumentException ex)
      {
        return BadRequest(ex.Message);
      }
      catch (Exception)
      {
        // log real exception in production
        return StatusCode(500, "An error occurred while creating the doctor.");
      }
    }
    [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorRequest request)
        {
            if (request == null) return BadRequest("Request body is empty.");

            try
            {
                // Pass createSpecialtyIfMissing = false to reject unknown specialty names.
                var updated = await _doctorRepository.UpdateDoctorAsync(id, request, createSpecialtyIfMissing: false);

                // map to DTO if needed; here return updated entity (avoid returning EF tracked entity in prod)
                return Ok(new
                {
                    id = updated.Id,
                    fullName = updated.FullName,
                    hospital = updated.Hospital,
                    specialtyId = updated.SpecialtyId,
                    specialtyName = updated.Specialty?.Name,
                    phone = updated.Phone,
                    avatarUrl = updated.AvatarUrl
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Database update failure
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor(Guid id)
    {
      var result = await _doctorRepository.DeleteDoctorAsync(id);
      if (!result)
      {
        return NotFound();
      }
      return NoContent();
    }
  }
}