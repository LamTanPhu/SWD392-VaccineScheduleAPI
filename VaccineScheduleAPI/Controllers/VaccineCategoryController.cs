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
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccineCategoryResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllCategoriesAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccineCategoryResponseDTO>> GetById(string id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(VaccineCategoryRequestDTO categoryDto)
        {
            await _service.AddCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new { id = categoryDto.CategoryName }, categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, VaccineCategoryRequestDTO categoryDto)
        {
            await _service.UpdateCategoryAsync(id, categoryDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
