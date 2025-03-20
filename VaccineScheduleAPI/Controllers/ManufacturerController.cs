using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Manufacturer;
using ModelViews.Responses.Manufacturer;
using IServices.Interfaces.Inventory;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService ?? throw new ArgumentNullException(nameof(manufacturerService));
        }

        [HttpGet]
        public async Task<ActionResult<IList<ManufacturerResponseDto>>> GetAllManufacturers()
        {
            try
            {
                var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
                return Ok(manufacturers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerResponseDto>> GetManufacturerById(string id)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                if (manufacturer == null)
                    return NotFound(new { message = $"Manufacturer with ID {id} not found." });

                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("byname/{name}")]
        public async Task<ActionResult<ManufacturerResponseDto>> GetManufacturerByName(string name)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByNameAsync(name);
                if (manufacturer == null)
                    return NotFound(new { message = $"Manufacturer with name {name} not found." });

                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ManufacturerResponseDto>> CreateManufacturer([FromBody] ManufacturerRequestDto manufacturerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdManufacturer = await _manufacturerService.AddManufacturerAsync(manufacturerDto);
                return CreatedAtAction(nameof(GetManufacturerById), new { id = createdManufacturer.Id }, createdManufacturer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateManufacturer(string id, [FromBody] ManufacturerRequestDto manufacturerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _manufacturerService.UpdateManufacturerAsync(id, manufacturerDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteManufacturer(string id)
        {
            try
            {
                await _manufacturerService.DeleteManufacturerAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}