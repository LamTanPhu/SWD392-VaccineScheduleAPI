using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IServices.Interfaces;
using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;

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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteVaccineCenter(string id)
        //{

        //    try
        //    {
        //        await _vaccineCenterService.DeleteAsync(id);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
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

        [HttpGet("{name}")]
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
