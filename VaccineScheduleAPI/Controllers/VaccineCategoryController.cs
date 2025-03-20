using IServices.Interfaces.Vaccines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineCategoryController : ControllerBase
    {
        private readonly IVaccineCategoryService _service;

        public VaccineCategoryController(IVaccineCategoryService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineCategoryResponseDTO>>> GetAll()
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineCategoryResponseDTO>> GetById(string id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
                return NotFound($"Vaccine category with ID {id} not found.");
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccineCategoryResponseDTO>> Create([FromBody] VaccineCategoryRequestDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCategory = await _service.CreateAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] VaccineCategoryRequestDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id, categoryDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}