using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Threading.Tasks;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using Microsoft.AspNetCore.Authorization;
using IServices.Interfaces.Accounts;
using IRepositories.Entity.Accounts;
using Microsoft.AspNetCore.Identity.Data;
using ModelViews.Requests.Mail;
using IServices.Interfaces.Mail;
using Org.BouncyCastle.Crypto.Generators;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthenticationController(IAccountService accountService, IJwtService jwtService, IEmailService emailService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        // POST api/authentication/register
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO request)
        {
            var response = await _accountService.RegisterAsync(request);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        // POST api/authentication/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
        {
            var response = await _accountService.LoginAsync(request);
            if (string.IsNullOrEmpty(response.Token))
                return Unauthorized(new { message = "Invalid username or password." });
            return Ok(response); // Return the successful login response
        }

        // GET api/authentication/profile
        [Authorize(Roles = "Admin, Staff, Parent")]
        [HttpGet("profile")]
        public async Task<ActionResult<Account>> GetProfileAsync()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Token is required" });

            var account = await _jwtService.ExtractAccountAsync(token);
            if (account == null)
                return Unauthorized(new { message = "Invalid token" });

            return Ok(account);
        }

        // POST api/authentication/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            var user = await _accountService.GetUserByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Generate OTP and set expiry time (10 minutes)
            var otp = new Random().Next(100000, 999999).ToString();
            user.OTP = otp;
            user.OTPExpired = DateTime.UtcNow.AddMinutes(5); 

            await _accountService.UpdateUserAsync(user);

            // Send OTP via email
            var mailRequest = new Mailrequest
            {
                ToEmail = request.Email,
                Subject = "Password Reset OTP",
                Body = $"Your OTP for password reset is: {otp} (Valid for 10 minutes)"
            };
            await _emailService.SendEmailAsync(mailRequest);

            return Ok(new { message = "OTP has been sent to your email." });
        }


        // POST api/authentication/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _accountService.GetUserByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Validate OTP and expiry time
            if (user.OTP != request.Otp || user.OTPExpired == null || user.OTPExpired < DateTime.UtcNow)
                return BadRequest(new { message = "Invalid or expired OTP." });

            // Reset password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.OTP = null;
            user.OTPExpired = null; // Clear OTP after use
            await _accountService.UpdateUserAsync(user);

            return Ok(new { message = "Password has been reset successfully." });
        }

    }
}
