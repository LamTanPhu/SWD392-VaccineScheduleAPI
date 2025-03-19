using IServices.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Responses.VaccineBatch;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineBatchController : ControllerBase
    {
        private readonly IVaccineBatchService _vaccineBatchService;

        public VaccineBatchController(IVaccineBatchService vaccineBatchService)
        {
            _vaccineBatchService = vaccineBatchService ?? throw new ArgumentNullException(nameof(vaccineBatchService));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineBatchResponseDTO>>> GetAll()
        {
            var batches = await _vaccineBatchService.GetAllAsync();
            return Ok(batches);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{batchNumber}")]
        public async Task<ActionResult<VaccineBatchResponseDTO>> GetByBatchNumber(string batchNumber)
        {
            var batch = await _vaccineBatchService.GetByBatchNumberAsync(batchNumber);
            if (batch == null)
                return NotFound($"Vaccine batch with number {batchNumber} not found.");
            return Ok(batch);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<VaccineBatchResponseDTO>>> SearchByName(string name)
        {
            var batches = await _vaccineBatchService.SearchByNameAsync(name);
            return Ok(batches);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineBatchResponseDTO>> Create([FromBody] AddVaccineBatchRequestDTO batchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBatch = await _vaccineBatchService.CreateAsync(batchDto);
            return CreatedAtAction(nameof(GetByBatchNumber), new { batchNumber = createdBatch.BatchNumber }, createdBatch);
        }


    }
}