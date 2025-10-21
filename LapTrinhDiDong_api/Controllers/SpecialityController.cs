using LapTrinhDiDong_api.Models;
using LapTrinhDiDong_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinhDiDong_api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class SpecialityController : ControllerBase
  {
    private readonly ISpecialityRepository _specialityRepository;

    public SpecialityController(ISpecialityRepository specialityRepository)
    {
      _specialityRepository = specialityRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Speciality>> GetSpecialityById(Guid id)
    {
      if (id == Guid.Empty)
        return BadRequest("ID không hợp lệ");

      var speciality = await _specialityRepository.GetSpecialityByIdAsync(id);
      if (speciality == null)
        return NotFound();

      return Ok(speciality);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Speciality>>> GetAllSpecialities()
    {
      var result = await _specialityRepository.GetAllSpecialitiesAsync();
      return Ok(result);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Speciality>> CreateSpeciality([FromBody] Speciality speciality)
    {
      if (speciality == null || string.IsNullOrWhiteSpace(speciality.Name))
        return BadRequest("Dữ liệu không hợp lệ");

      var created = await _specialityRepository.CreateSpecialityAsync(speciality);
      return CreatedAtAction(nameof(GetSpecialityById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Speciality>> UpdateSpeciality(Guid id, [FromBody] Speciality speciality)
    {
      if (id == Guid.Empty || speciality == null || speciality.Id == Guid.Empty || string.IsNullOrWhiteSpace(speciality.Name))
        return BadRequest("Dữ liệu không hợp lệ");

      if (id != speciality.Id)
        return BadRequest("Id mismatch!");

      var updated = await _specialityRepository.UpdateSpecialityAsync(speciality);
      return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
      if (id == Guid.Empty)
        return BadRequest("ID không hợp lệ");

      await _specialityRepository.DeleteSpecialityAsync(id);
      return NoContent();
    }
  }
}