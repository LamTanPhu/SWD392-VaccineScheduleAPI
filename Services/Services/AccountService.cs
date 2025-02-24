using BCrypt.Net;
using IRepositories.Entity;
using IRepositories.Enum;
using IRepositories.IRepository;
using IServices.Interfaces;
using ModelViews.DTOs;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;

        public AccountService(IAccountRepository accountRepository, IJwtService jwtService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _accountRepository.GetByUsernameAsync(username);
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var existingUserByEmail = await _accountRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                return new RegisterResponseDTO
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var existingUserByUsername = await _accountRepository.GetByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                return new RegisterResponseDTO
                {
                    Success = false,
                    Message = "Username already exists."
                };
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new Account
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = RoleEnum.Parent, // Default role, change based on your logic
                Status = "Active"
            };

            var success = await _accountRepository.AddUserAsync(newUser);
            if (success)
            {
                return new RegisterResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully."
                };
            }

            return new RegisterResponseDTO
            {
                Success = false,
                Message = "Registration failed."
            };
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _accountRepository.GetByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponseDTO
                {
                    Username = request.Username,
                    Role = RoleEnum.Parent, // Default or adjust depending on your logic
                    Token = string.Empty, // No token if login failed
                    Expiration = DateTime.MinValue // No expiration if login failed
                };
            }

            var token = _jwtService.GenerateJwtToken(user);
            var expiration = _jwtService.ExtractExpiration(token);

            return new LoginResponseDTO
            {
                Username = user.Username,
                Role = user.Role,
                Token = token,
                Expiration = expiration
            };
        }
    }
}
