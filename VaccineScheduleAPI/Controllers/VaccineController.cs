using IServices.Interfaces.Vaccines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Vaccine;
using ModelViews.Responses.Vaccine;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineController : ControllerBase
    {
        private readonly IVaccineService _service;

        public VaccineController(IVaccineService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineResponseDTO>>> GetAll()
        {
            var vaccines = await _service.GetAllVaccinesAsync();
            return Ok(vaccines);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineResponseDTO>> GetById(string id)
        {
            var vaccine = await _service.GetVaccineByIdAsync(id);
            if (vaccine == null) return NotFound();
            return Ok(vaccine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineResponseDTO>> Create([FromForm] VaccineRequestDTO vaccineDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdVaccine = await _service.AddVaccineAsync(vaccineDto);
            return CreatedAtAction(nameof(GetById), new { id = createdVaccine.Id }, createdVaccine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<VaccineResponseDTO>> Update(string id, [FromForm] VaccineRequestDTO vaccineDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedVaccine = await _service.UpdateVaccineAsync(id, vaccineDto);
            if (updatedVaccine == null)
                return NotFound();

            return Ok(updatedVaccine); 
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteVaccineAsync(id);
            return NoContent();
        }
    }
}
