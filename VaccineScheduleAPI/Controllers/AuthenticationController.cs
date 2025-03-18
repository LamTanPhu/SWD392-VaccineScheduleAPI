using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using IServices.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using ModelViews.Requests.Forgot_Password;
using ModelViews.Responses.Forgot_Password;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRegistrationService _registrationService; // Added for /register

        public AuthenticationController(
            IAuthService authService,
            IRegistrationService registrationService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterAsync([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid request data.", Errors = ModelState });

            var response = await _registrationService.RegisterAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> LoginAsync([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid request data.", Errors = ModelState });

            var response = await _authService.LoginAsync(request);
            if (string.IsNullOrEmpty(response.Token))
                return Unauthorized(new { Message = "Invalid username or password." });

            return Ok(response);
        }

        [HttpPost("login-with-google")]
        public async Task<ActionResult<LoginResponseDTO>> LoginWithGoogleAsync([FromBody] GoogleLoginRequestDTO request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.TokenId))
                return BadRequest(new { Message = "Google token is required." });

            var response = await _authService.LoginWithGoogleAsync(request.TokenId);
            if (string.IsNullOrEmpty(response.Token))
                return Unauthorized(new { Message = "Invalid Google token." });

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponseDTO>> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid request data.", Errors = ModelState });

            var response = await _authService.ForgotPasswordAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("verify-reset")]
        public async Task<ActionResult<VerifyResetResponseDTO>> VerifyReset([FromQuery] string token)
        {
            var request = new VerifyResetRequestDTO { Token = token };
            var response = await _authService.VerifyResetAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("reset-password")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<ResetPasswordResponseDTO>> ResetPasswordAsync([FromBody] ResetPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid request data.", Errors = ModelState });

            var response = await _authService.ResetPasswordAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }


    }
}