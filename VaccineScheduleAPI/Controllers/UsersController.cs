using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IServices.Interfaces.Accounts;
using ModelViews.Responses.Auth;
using ModelViews.Requests.Auth;
using System.Threading.Tasks;

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
            IAccountUpdateService accountUpdateService,
            IJwtService jwtService)
        {
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _accountUpdateService = accountUpdateService ?? throw new ArgumentNullException(nameof(accountUpdateService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDTO>> GetProfileAsync()
        {
            try
            {
                var profile = await _userProfileService.GetProfileAsync(); // Đẩy logic lấy email và kiểm tra token xuống service
                if (profile == null)
                    return NotFound();

                return Ok(profile);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Chỉ catch lỗi HTTP, ném exception khác ra ngoài
            }
        }

        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<ProfileResponseDTO>> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            try
            {
                var profile = await _userProfileService.GetProfileByEmailAsync(email);
                if (profile == null)
                    return NotFound();

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpPut("update-profile")]
        public async Task<ActionResult<ProfileResponseDTO>> UpdateProfile([FromBody] UpdateAccountRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedProfile = await _accountUpdateService.UpdateAccountAsync(request); // Đẩy logic lấy email và kiểm tra token xuống service
                if (updatedProfile == null)
                    return NotFound();

                return Ok(updatedProfile);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}