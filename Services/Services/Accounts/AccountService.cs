using BCrypt.Net;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.DTOs;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IFirebaseAuthService _firebaseAuthService; // New dependency for Firebase token validation

        public AccountService(IAccountRepository accountRepository, IJwtService jwtService, IFirebaseAuthService firebaseAuthService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _firebaseAuthService = firebaseAuthService; // Inject Firebase service

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
                Role = request.Role,  // Use the role passed in the request
                Status = "Active"
            };

            var success = await _accountRepository.AddUserAsync(newUser);
            if (success)
            {
                return new RegisterResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Role = newUser.Role  // Include the role in the response
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
                    Role = RoleEnum.Parent,  // Default role on failure
                    Token = string.Empty,
                    Expiration = DateTime.MinValue
                };
            }

            var token = _jwtService.GenerateJwtToken(user);
            var expiration = _jwtService.ExtractExpiration(token);

            return new LoginResponseDTO
            {
                Username = user.Username,
                Role = user.Role,  // Return the correct role from the database
                Token = token,
                Expiration = expiration
            };
        }

        public async Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken)
        {
            // Verify Firebase ID token
            var payload = await _firebaseAuthService.VerifyFirebaseTokenAsync(idToken);

            // Find the user in your database by email
            var user = await _accountRepository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                // Optionally, you can auto-register the user if not found
                user = new Account
                {
                    Username = payload.Email,
                    Email = payload.Email,
                    Role = RoleEnum.Parent,
                    Status = "Active"
                };
                await _accountRepository.AddUserAsync(user);
            }

            // Generate JWT token for the user
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
