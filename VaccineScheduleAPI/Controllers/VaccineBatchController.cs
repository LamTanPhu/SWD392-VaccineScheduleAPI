using IServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Responses.VaccineBatch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineBatchController : ControllerBase
    {
        private readonly IVaccineBatchService _vaccineBatchService;

        public VaccineBatchController(IVaccineBatchService vaccineBatchService)
        {
            _vaccineBatchService = vaccineBatchService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineBatchResponseDTO>>> GetAllVaccineBatches()
        {
            try
            {
                var batches = await _vaccineBatchService.SearchByNameAsync("");
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{batchNumber}")]
        public async Task<ActionResult<VaccineBatchResponseDTO>> GetVaccineBatchByNumber(string batchNumber)
        {
            try
            {
                var batch = await _vaccineBatchService.GetByBatchNumberAsync(batchNumber);
                if (batch == null)
                {
                    return NotFound($"Vaccine batch with number {batchNumber} not found.");
                }
                return Ok(batch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<IEnumerable<VaccineBatchResponseDTO>>> SearchVaccineBatchByName(string name)
        {
            try
            {
                var batches = await _vaccineBatchService.SearchByNameAsync(name);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddVaccineBatch([FromBody] AddVaccineBatchRequestDTO batchDto)
        {
            if (batchDto == null)
            {
                return BadRequest("Vaccine batch data is null.");
            }

            try
            {
                var response = await _vaccineBatchService.AddBatchAsync(batchDto);
                if (!response.Success)
                {
                    return BadRequest(response.Message);
                }
                return CreatedAtAction(nameof(GetVaccineBatchByNumber), new { batchNumber = response.BatchNumber }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
