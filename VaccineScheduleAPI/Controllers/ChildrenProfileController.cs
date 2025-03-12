using IServices.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Responses.ChildrenProfile;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildrenProfileController : ControllerBase
    {

        private readonly IChildrenProfileService _service;

        public ChildrenProfileController(IChildrenProfileService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChildrenProfileResponseDTO>>> GetAll()
        {
            return Ok(await _service.GetAllProfilesAsync());
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ChildrenProfileResponseDTO>> GetById(string id)
        {
            var profile = await _service.GetProfileByIdAsync(id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(ChildrenProfileRequestDTO profileDto)
        {
            await _service.AddProfileAsync(profileDto);
            return CreatedAtAction(nameof(GetById), new { id = profileDto.FullName }, profileDto);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ChildrenProfileRequestDTO profileDto)
        {
            await _service.UpdateProfileAsync(id, profileDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}

