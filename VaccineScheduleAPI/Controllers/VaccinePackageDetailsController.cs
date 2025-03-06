using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IServices.Interfaces.Vaccines;
using ModelViews.Responses.VaccinePackage;
using ModelViews.Requests.VaccinePackage;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinePackageDetailsController : ControllerBase
    {
        private readonly IVaccinePackageDetailsService _service;

        public VaccinePackageDetailsController(IVaccinePackageDetailsService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccinePackageDetailsResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllDetailsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccinePackageDetailsResponseDTO>> GetById(string id)
        {
            var detail = await _service.GetDetailByIdAsync(id);
            if (detail == null) return NotFound();
            return Ok(detail);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create(VaccinePackageDetailsRequestDTO detailDto)
        {
            await _service.AddDetailAsync(detailDto);
            return CreatedAtAction(nameof(GetById), new { id = detailDto.VaccineId }, detailDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, VaccinePackageDetailsRequestDTO detailDto)
        {
            await _service.UpdateDetailAsync(id, detailDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteDetailAsync(id);
            return NoContent();
        }
    }
}
