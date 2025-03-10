using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IServices.Interfaces.Vaccines;
namespace VaccineScheduleAPI.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class VaccinePackageController : ControllerBase
        {
        private readonly IVaccinePackageService _service;

        public VaccinePackageController(IVaccinePackageService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VaccinePackageResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllPackagesAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccinePackageResponseDTO>> GetById(string id)
        {
            var package = await _service.GetPackageByIdAsync(id);
            if (package == null) return NotFound();
            return Ok(package);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccinePackageResponseDTO>> Create([FromBody] VaccinePackageRequestDTO packageDto)
        {
            var createdPackage = await _service.AddPackageAsync(packageDto);
            return CreatedAtAction(nameof(GetById), new { id = createdPackage.Id }, createdPackage);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] VaccinePackageRequestDTO packageDto)
        {
            await _service.UpdatePackageAsync(id, packageDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/add-vaccine")]
        public async Task<ActionResult> AddVaccineToPackage(string id, [FromBody] VaccinePackageUpdateRequestDTO request)
        {
            await _service.AddVaccineToPackageAsync(id, request);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/remove-vaccine")]
        public async Task<ActionResult> RemoveVaccineFromPackage(string id, [FromBody] VaccinePackageUpdateRequestDTO request)
        {
            await _service.RemoveVaccineFromPackageAsync(id, request);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeletePackageAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-vaccines-and-packages")]
        public async Task<ActionResult<CombinedVaccineResponseDTO>> GetAllVaccinesAndPackages()
        {
            return Ok(await _service.GetAllVaccinesAndPackagesAsync());
        }
    }
    }
