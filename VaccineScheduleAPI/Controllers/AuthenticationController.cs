using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IServices.Interfaces;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using Microsoft.AspNetCore.Authorization;
using IRepositories.Entity;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;

        public AuthenticationController(IAccountService accountService, IJwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
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

    }
}
