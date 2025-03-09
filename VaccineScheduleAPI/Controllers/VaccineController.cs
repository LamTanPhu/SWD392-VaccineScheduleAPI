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
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllVaccinesAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineResponseDTO>> GetById(string id)
        {
            var vaccine = await _service.GetVaccineByIdAsync(id);
            if (vaccine == null) return NotFound();
            return Ok(vaccine);
        }

        // ✅ Modified to accept VaccineRequestDTO + Image file
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineResponseDTO>> Create([FromForm] VaccineRequestDTO vaccineDto)
        {
            var createdVaccine = await _service.AddVaccineAsync(vaccineDto);
            return CreatedAtAction(nameof(GetById), new { id = createdVaccine.Id }, createdVaccine);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, VaccineRequestDTO vaccineDto)
        {
            await _service.UpdateVaccineAsync(id, vaccineDto);
            return NoContent();
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
