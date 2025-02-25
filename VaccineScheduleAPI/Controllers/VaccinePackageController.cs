using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using ModelViews.Requests.VaccinePackage;
    using ModelViews.Responses.VaccinePackage;
    using System.Collections.Generic;
    using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace VaccineScheduleAPI.Controllers
{


        [Route("api/[controller]")]
        [ApiController]
        public class VaccinePackageController : ControllerBase
        {
            private readonly IVaccinePackageService _service;

            public VaccinePackageController(IVaccinePackageService service)
            {
                _service = service;
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
            public async Task<ActionResult> Create(VaccinePackageRequestDTO packageDto)
            {
                await _service.AddPackageAsync(packageDto);
                return CreatedAtAction(nameof(GetById), new { id = packageDto.PackageName }, packageDto);
            }

            [Authorize(Roles = "Admin")]
            [HttpPut("{id}")]
            public async Task<ActionResult> Update(string id, VaccinePackageRequestDTO packageDto)
            {
                await _service.UpdatePackageAsync(id, packageDto);
                return NoContent();
            }

            [Authorize(Roles = "Admin")]
            [HttpDelete("{id}")]
            public async Task<ActionResult> Delete(string id)
            {
                await _service.DeletePackageAsync(id);
                return NoContent();
            }
        }
    }
