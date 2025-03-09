using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using IServices.Interfaces.Accounts;

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
        public async Task<ActionResult<Account>> GetProfileAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { Message = "Token is required." });

            var account = await _jwtService.ExtractAccountAsync(token);
            if (account == null || account.DeletedTime != null)
                return Unauthorized(new { Message = "Invalid token or account deleted." });

            return Ok(account);
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