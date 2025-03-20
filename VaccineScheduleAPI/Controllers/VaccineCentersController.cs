using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using IServices.Interfaces.Inventory;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineCentersController : ControllerBase
    {
        private readonly IVaccineCenterService _vaccineCenterService;

        public VaccineCentersController(IVaccineCenterService vaccineCenterService)
        {
            _vaccineCenterService = vaccineCenterService ?? throw new ArgumentNullException(nameof(vaccineCenterService));
        }

        [HttpGet]
        public async Task<ActionResult<IList<VaccineCenterResponseDTO>>> GetAllVaccineCenters()
        {
            try
            {
                var vaccineCenters = await _vaccineCenterService.GetAllAsync();
                return Ok(vaccineCenters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("public")]
        public async Task<ActionResult<IList<object>>> GetAllVaccineCentersPublic()
        {
            try
            {
                var filteredCenters = await _vaccineCenterService.GetAllPublicAsync();
                return Ok(filteredCenters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineCenterResponseDTO>> GetVaccineCenterById(string id)
        {
            try
            {
                var vaccineCenter = await _vaccineCenterService.GetByIdAsync(id);
                if (vaccineCenter == null)
                    return NotFound(new { message = $"Vaccine center with ID {id} not found." });

                return Ok(vaccineCenter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineCenterResponseDTO>> CreateVaccineCenter([FromBody] VaccineCenterRequestDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdVaccineCenter = await _vaccineCenterService.AddAsync(model);
                return CreatedAtAction(nameof(GetVaccineCenterById), new { id = createdVaccineCenter.Id }, createdVaccineCenter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVaccineCenter(string id, [FromBody] VaccineCenterUpdateDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _vaccineCenterService.UpdateAsync(id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVaccineCenter(string id)
        {
            try
            {
                await _vaccineCenterService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("byname/{name}")]
        public async Task<ActionResult<IList<VaccineCenterResponseDTO>>> GetVaccineCentersByName(string name)
        {
            try
            {
                var vaccineCenters = await _vaccineCenterService.GetByNameAsync(name);
                if (vaccineCenters.Count == 0)
                    return NotFound(new { message = "No matching vaccine centers found." });

                return Ok(vaccineCenters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}