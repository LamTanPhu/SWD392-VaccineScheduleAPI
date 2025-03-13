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
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<ChildrenProfileResponseDTO>>> GetAllByAccountId(string accountId)
        {
            // For Parents, ensure they can only access their own children's profiles
            if (User.IsInRole("Parent"))
            {
                var userAccountId = User.FindFirst("AccountId")?.Value;
                if (string.IsNullOrEmpty(userAccountId) || userAccountId != accountId)
                {
                    return Forbid("You can only access your own children's profiles.");
                }
            }

            var profiles = await _service.GetAllProfilesByAccountIdAsync(accountId);
            return Ok(profiles);
        }

        [Authorize(Roles = "Admin, Parent")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ChildrenProfileResponseDTO>> GetById(string id)
        {
            var profile = await _service.GetProfileByIdAsync(id);
            if (profile == null) return NotFound();

            // For Parents, ensure they can only access their own children's profiles
            if (User.IsInRole("Parent"))
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (profile.AccountId != accountId)
                {
                    return Forbid("You can only access your own children's profiles.");
                }
            }

            return Ok(profile);
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(ChildrenProfileRequestDTO profileDto)
        {
            // Ensure the AccountId in the DTO matches the logged-in user
            var accountId = User.FindFirst("AccountId")?.Value;
            if (profileDto.AccountId != accountId)
            {
                return Forbid("You can only create profiles for your own account.");
            }

            await _service.AddProfileAsync(profileDto);
            return CreatedAtAction(nameof(GetById), new { id = profileDto.FullName }, profileDto);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ChildrenProfileRequestDTO profileDto)
        {
            var existingProfile = await _service.GetProfileByIdAsync(id);
            if (existingProfile == null) return NotFound();

            // Ensure the profile belongs to the logged-in user
            var accountId = User.FindFirst("AccountId")?.Value;
            if (existingProfile.AccountId != accountId)
            {
                return Forbid("You can only update your own children's profiles.");
            }

            await _service.UpdateProfileAsync(id, profileDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingProfile = await _service.GetProfileByIdAsync(id);
            if (existingProfile == null) return NotFound();

            // Ensure the profile belongs to the logged-in user
            var accountId = User.FindFirst("AccountId")?.Value;
            if (existingProfile.AccountId != accountId)
            {
                return Forbid("You can only delete your own children's profiles.");
            }

            await _service.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}