using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
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

        [HttpGet]
        public async Task<ActionResult<IList<VaccinePackageResponseDTO>>> GetAll()
        {
            try
            {
                var packages = await _service.GetAllPackagesAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<VaccinePackageResponseDTO>> GetById(string id)
        {
            try
            {
                var package = await _service.GetPackageByIdAsync(id);
                if (package == null)
                    return NotFound(new { message = $"Vaccine package with ID {id} not found." });
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<VaccinePackageResponseDTO>> Create([FromBody] VaccinePackageRequestDTO packageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdPackage = await _service.AddPackageAsync(packageDto);
                return CreatedAtAction(nameof(GetById), new { id = createdPackage.Id }, createdPackage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<VaccinePackageResponseDTO>> Update(string id, [FromBody] VaccinePackageRequestDTO packageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var package = await _service.UpdatePackageAsync(id, packageDto);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/add-vaccine")]
        public async Task<ActionResult> AddVaccineToPackage(string id, [FromBody] VaccinePackageUpdateRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.AddVaccineToPackageAsync(id, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/remove-vaccine")]
        public async Task<ActionResult> RemoveVaccineFromPackage(string id, [FromBody] VaccinePackageUpdateRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.RemoveVaccineFromPackageAsync(id, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _service.DeletePackageAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-vaccines-and-packages")]
        public async Task<ActionResult<CombinedVaccineResponseDTO>> GetAllVaccinesAndPackages()
        {
            try
            {
                var result = await _service.GetAllVaccinesAndPackagesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}