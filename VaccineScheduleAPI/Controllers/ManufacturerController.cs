using IServices.Interfaces.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.Manufacturer;
using ModelViews.Responses.Manufacturer;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        // Get all manufacturers
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManufacturerResponseDto>>> GetAllManufacturers()
        {
            try
            {
                var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
                return Ok(manufacturers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get manufacturer by ID
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerResponseDto>> GetManufacturerById(string id)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                if (manufacturer == null)
                {
                    return NotFound($"Manufacturer with ID {id} not found.");
                }
                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get manufacturer by name
        [Authorize(Roles = "Admin")]
        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<ManufacturerResponseDto>> GetManufacturerByName(string name)
        {
            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByNameAsync(name);
                if (manufacturer == null)
                {
                    return NotFound($"Manufacturer with name {name} not found.");
                }
                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add new manufacturer
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddManufacturer([FromBody] ManufacturerRequestDto manufacturerDto)
        {
            if (manufacturerDto == null)
            {
                return BadRequest("Manufacturer data is null.");
            }

            try
            {
                await _manufacturerService.AddManufacturerAsync(manufacturerDto);
                return CreatedAtAction(nameof(GetManufacturerById), new { id = manufacturerDto.Name }, manufacturerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Update existing manufacturer
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateManufacturer(string id, [FromBody] ManufacturerRequestDto manufacturerDto)
        {
            if (manufacturerDto == null)
            {
                return BadRequest("Manufacturer data is null.");
            }

            try
            {
                await _manufacturerService.UpdateManufacturerAsync(id, manufacturerDto);
                return NoContent();  // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Delete manufacturer
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteManufacturer(string id)
        {
            try
            {
                await _manufacturerService.DeleteManufacturerAsync(id);
                return NoContent();  // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}