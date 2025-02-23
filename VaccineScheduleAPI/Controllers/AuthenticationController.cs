using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IServices.Interfaces;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using Core.Utils;

namespace VaccineScheduleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthenticationController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { message = "Invalid login request." });
                }

                var user = await _accountService.GetByUsernameAsync(model.Username);
                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                return Ok(new LoginResponseDTO { Username = user.Username, Role = user.Role });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    exception = ex.ToString() // Returns full exception details
                });
            }
        }
        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
{
    try
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
        {
            return BadRequest(new { message = "Email, Password, and ConfirmPassword are required." });
        }

        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest(new { message = "Password and ConfirmPassword do not match." });
        }

        model.Password = CoreHelper.HashPassword(model.Password);
        var result = await _accountService.RegisterAsync(model);
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Registration successful." });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            message = ex.Message,
            exception = ex.ToString()
        });
    }
}


        private bool VerifyPassword(string password, string storedHash)
        {
            return CoreHelper.HashPassword(password) == storedHash;
        }
    }
}
