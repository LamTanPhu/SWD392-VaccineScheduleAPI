using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using Microsoft.AspNetCore.Authorization;
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
            _vaccineCenterService = vaccineCenterService;
        }

        // Allow any authenticated user to access this method
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllVaccineCenters()
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

        // Public endpoint for fetching vaccine centers (no authentication required)
        //[AllowAnonymous] // Uncommented to allow public access
        [HttpGet("public")]
        public async Task<IActionResult> GetAllVaccineCentersPublic()
        {
            try
            {
                var vaccineCenters = await _vaccineCenterService.GetAllAsync();
                // Return only necessary fields (id and name) for public access
                var filteredCenters = vaccineCenters
                    .Select(vc => new
                    {
                        id = vc.Id,
                        name = vc.Name
                    })
                    .ToList();
                return Ok(filteredCenters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Only Admin can access this method
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaccineCenterById(string id)
        {
            try
            {
                var vaccineCenter = await _vaccineCenterService.GetByIdAsync(id);
                if (vaccineCenter == null)
                {
                    return NotFound(new { message = "Vaccine center not found." });
                }
                return Ok(vaccineCenter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Only Admin can access this method
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateVaccineCenter([FromBody] VaccineCenterRequestDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { message = "Vaccine center data is required." });
                }
                var createdVaccineCenter = await _vaccineCenterService.AddAsync(model);
                return CreatedAtAction(nameof(GetVaccineCenterById), new { id = createdVaccineCenter.Id }, createdVaccineCenter);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Only Admin can access this method
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVaccineCenter(string id, [FromBody] VaccineCenterUpdateDTO model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest(new { message = "Vaccine center ID mismatch." });
                }
                await _vaccineCenterService.UpdateAsync(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Only Admin can access this method
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteVaccineCenter([FromBody] VaccineCenterDeleteDTO model)
        {
            try
            {
                await _vaccineCenterService.DeleteAsync(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Only Admin can access this method
        [Authorize(Roles = "Admin")]
        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetVaccineCentersByName(string name)
        {
            try
            {
                var vaccineCenters = await _vaccineCenterService.GetByNameAsync(name);
                if (vaccineCenters.Count == 0)
                {
                    return NotFound(new { message = "No matching vaccine centers found." });
                }
                return Ok(vaccineCenters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
