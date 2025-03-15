﻿using IServices.Interfaces.Accounts;
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
        private readonly IUserProfileService _userProfileService;
        private readonly IChildrenProfileService _childrenProfileService;

        public ChildrenProfileController(IUserProfileService userProfileService, IChildrenProfileService childrenProfileService)
        {
            _userProfileService = userProfileService;
            _childrenProfileService = childrenProfileService;
        }

        [Authorize(Roles = "Parent")]
        [HttpGet("my-children")]
        public async Task<ActionResult<IEnumerable<ChildrenProfileResponseDTO>>> GetMyChildrenAsync()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            var profile = await _userProfileService.GetProfileByUsernameAsync(username);
            if (profile == null)
                return NotFound(new { Message = "User profile not found." });

            return Ok(profile.ChildrenProfiles ?? new List<ChildrenProfileResponseDTO>());
        }

        [Authorize(Roles = "Parent")]
        [HttpPost]
        public async Task<ActionResult> Create(ChildrenProfileRequestDTO profileDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            var account = await _userProfileService.GetByUsernameAsync(username);
            if (account == null)
                return NotFound(new { Message = "User not found." });

            profileDto.AccountId = account.Id; // Set the AccountId from the authenticated user
            await _childrenProfileService.AddProfileAsync(profileDto);
            return CreatedAtAction(nameof(GetMyChildrenAsync), new { id = profileDto.FullName }, profileDto);
        }

        [Authorize(Roles = "Parent")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, ChildrenProfileRequestDTO profileDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            var existingProfile = await _childrenProfileService.GetProfileByIdAsync(id);
            if (existingProfile == null)
                return NotFound();

            if (existingProfile.AccountId != (await _userProfileService.GetByUsernameAsync(username))?.Id)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only update your own children's profiles." });

            await _childrenProfileService.UpdateProfileAsync(id, profileDto);
            return NoContent();
        }

        [Authorize(Roles = "Parent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            var existingProfile = await _childrenProfileService.GetProfileByIdAsync(id);
            if (existingProfile == null)
                return NotFound();

            if (existingProfile.AccountId != (await _userProfileService.GetByUsernameAsync(username))?.Id)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only delete your own children's profiles." });

            await _childrenProfileService.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}
