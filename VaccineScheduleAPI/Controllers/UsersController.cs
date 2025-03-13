using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using IServices.Interfaces.Accounts;
using System.Security.Claims;
using ModelViews.Responses.Auth;
using ModelViews.Requests.Auth;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAccountUpdateService _accountUpdateService;

        private readonly IJwtService _jwtService;

        public UsersController(
            IUserProfileService userProfileService,
            IJwtService jwtService)
        {
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDTO>> GetProfileAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"Raw Authorization Header: '{authHeader}'");

            if (string.IsNullOrEmpty(authHeader))
                return Unauthorized(new { Message = "Token is required." });

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            Console.WriteLine($"Middleware Extracted Username: '{username}'");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            var token = authHeader.Trim();
            var expired = _jwtService.IsTokenExpired(token);
            Console.WriteLine($"Token Expired: {expired}, Expiration: {_jwtService.ExtractExpiration(token)}, Now: {DateTime.UtcNow}");
            if (expired)
                return Unauthorized(new { Message = "Token has expired." });

            var profileData = await _userProfileService.GetProfileByUsernameAsync(username);
            if (profileData == null)
                return NotFound(new { Message = "User not found or deleted." });

            var response = new ProfileResponseDTO
            {
                Username = profileData.Username,
                Email = profileData.Email ?? "Not provided",
                Role = profileData.Role,
                Status = profileData.Status ?? "Active",
                VaccineCenter = profileData.VaccineCenter,
                ChildrenProfiles = profileData.ChildrenProfiles
            };
            Console.WriteLine($"Final Response DTO: Username={response.Username}, Email={response.Email}, Status={response.Status}, Role={response.Role}");
            return new JsonResult(response); // Force DTO
        }

        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<Account>> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Message = "Email is required." });

            var user = await _userProfileService.GetUserByEmailAsync(email);
            if (user == null || user.DeletedTime != null)
                return NotFound(new { Message = "User not found or deleted." });

            return Ok(user);
        }


        [HttpPut("update-profile")]
        [Authorize] //will restrict roles later
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateAccountRequestDTO request)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized(new { Message = "Invalid token payload." });

                var updatedProfile = await _accountUpdateService.UpdateAccountAsync(username, request);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}