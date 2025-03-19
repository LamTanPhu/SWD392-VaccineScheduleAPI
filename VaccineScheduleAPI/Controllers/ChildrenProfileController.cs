using IServices.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelViews.Requests.ChildrenProfile;
using ModelViews.Responses.ChildrenProfile;
using System.Security.Claims;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildrenProfileController : ControllerBase
    {
        private readonly IChildrenProfileService _childrenProfileService;

        public ChildrenProfileController(IChildrenProfileService childrenProfileService)
        {
            _childrenProfileService = childrenProfileService ?? throw new ArgumentNullException(nameof(childrenProfileService));
        }

        [Authorize(Roles = "Parent")]
        [HttpGet("my-children")]
        public async Task<ActionResult<IEnumerable<ChildrenProfileResponseDTO>>> GetMyChildren()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token payload.");

            var profiles = await _childrenProfileService.GetMyChildrenProfilesAsync(email);
            return Ok(profiles);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult<ChildrenProfileResponseDTO>> Create([FromBody] ChildrenProfileCreateUpdateDTO profileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token payload.");

            var createdProfile = await _childrenProfileService.CreateProfileAsync(email, profileDto);
            return CreatedAtAction(nameof(GetMyChildren), new { id = createdProfile.Id }, createdProfile);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] ChildrenProfileCreateUpdateDTO profileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token payload.");

            await _childrenProfileService.UpdateProfileAsync(id, email, profileDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Invalid token payload.");

            await _childrenProfileService.DeleteProfileAsync(id, email);
            return NoContent();
        }
    }
}