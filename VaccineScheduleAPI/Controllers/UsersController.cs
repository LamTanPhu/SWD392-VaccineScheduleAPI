using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using IServices.Interfaces.Accounts;
using System.Security.Claims;
using ModelViews.Responses.Auth;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
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

            // Use the ClaimsPrincipal from the middleware (already validated)
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            Console.WriteLine($"Middleware Extracted Username: '{username}'");
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { Message = "Invalid token payload." });

            // Optional: Double-check with JwtService if needed
            var token = authHeader.Trim();
            var expired = _jwtService.IsTokenExpired(token);
            Console.WriteLine($"Token Expired: {expired}, Expiration: {_jwtService.ExtractExpiration(token)}, Now: {DateTime.UtcNow}");
            if (expired)
                return Unauthorized(new { Message = "Token has expired." });

            var profile = await _userProfileService.GetProfileByUsernameAsync(username);
            if (profile == null)
                return NotFound(new { Message = "User not found or deleted." });

            return Ok(profile);
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
    }
}