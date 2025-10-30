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
    private readonly ISpecialtyRepository _specialityRepository;

    public SpecialityController(ISpecialtyRepository specialityRepository)
    {
      _specialityRepository = specialityRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Specialty>> GetSpecialityById(Guid id)
    {
      if (id == Guid.Empty)
        return BadRequest("ID không hợp lệ");

      var speciality = await _specialityRepository.GetSpecialtyByIdAsync(id);
      if (speciality == null)
        return NotFound();

      return Ok(speciality);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Specialty>>> GetAllSpecialities()
    {
      var result = await _specialityRepository.GetAllSpecialtiesAsync();
      return Ok(result);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Specialty>> CreateSpeciality([FromBody] Specialty specialty)
    {
      if (specialty == null || string.IsNullOrWhiteSpace(specialty.Name))
        return BadRequest("Dữ liệu không hợp lệ");

      var created = await _specialityRepository.CreateSpecialtyAsync(specialty);
      return CreatedAtAction(nameof(GetSpecialityById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Specialty>> UpdateSpeciality(Guid id, [FromBody] Specialty specialty)
    {
      if (id == Guid.Empty || specialty == null || specialty.Id == Guid.Empty || string.IsNullOrWhiteSpace(specialty.Name))
        return BadRequest("Dữ liệu không hợp lệ");

      if (id != specialty.Id)
        return BadRequest("Id mismatch!");

      var updated = await _specialityRepository.UpdateSpecialtyAsync(id,specialty);
      return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpeciality(Guid id)
    {
      if (id == Guid.Empty)
        return BadRequest("ID không hợp lệ");

      await _specialityRepository.DeleteSpecialtyAsync(id);
      return NoContent();
    }
  }
}