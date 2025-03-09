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
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _firebaseAuthService = firebaseAuthService ?? throw new ArgumentNullException(nameof(firebaseAuthService));

        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _accountRepository.GetByUsernameAsync(username);
        }

        public async Task<Account?> GetUserByEmailAsync(string email)
        {
            return await _accountRepository.GetByEmailAsync(email);
        }

        public async Task UpdateUserAsync(Account user)
        {
            await _accountRepository.UpdateUserAsync(user);
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Registration request cannot be null.");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.ConfirmPassword))
                return new RegisterResponseDTO { Success = false, Message = "All fields are required." };

            if (request.Password != request.ConfirmPassword)
                return new RegisterResponseDTO { Success = false, Message = "Passwords do not match." };

            var existingUserByEmail = await _accountRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null && existingUserByEmail.DeletedTime == null)
                return new RegisterResponseDTO { Success = false, Message = "Email already exists." };

            var existingUserByUsername = await _accountRepository.GetByUsernameAsync(request.Username);
            if (existingUserByUsername != null && existingUserByUsername.DeletedTime == null)
                return new RegisterResponseDTO { Success = false, Message = "Username already exists." };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new Account
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = request.Role,
                Status = "Active",
                CreatedBy = "System", // Should be set to the registering user if available
                LastUpdatedBy = "System"
            };

            var success = await _accountRepository.AddUserAsync(newUser);
            if (success)
            {
                return new RegisterResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully.",
                    UserId = newUser.Id,
                    Role = newUser.Role
                };
            }

            return new RegisterResponseDTO { Success = false, Message = "Registration failed." };
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Login request cannot be null.");

            if (string.IsNullOrEmpty(request.UsernameOrEmail) || string.IsNullOrEmpty(request.Password))
                return new LoginResponseDTO
                {
                    Username = null,
                    Role = RoleEnum.Parent,
                    Token = string.Empty,
                    Expiration = DateTime.MinValue
                };

            var user = await _accountRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail);
            if (user == null || user.DeletedTime != null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponseDTO
                {
                    Username = null,
                    Role = RoleEnum.Parent, // Consider returning null or an error response instead
                    Token = string.Empty,
                    Expiration = DateTime.MinValue
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

        public async Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken)
        {
            if (string.IsNullOrEmpty(idToken))
                throw new ArgumentNullException(nameof(idToken), "Google ID token cannot be null or empty.");

            var payload = await _firebaseAuthService.VerifyFirebaseTokenAsync(idToken);
            var user = await _accountRepository.GetByEmailAsync(payload.Email);
            if (user == null || user.DeletedTime != null)
            {
                user = new Account
                {
                    Username = $"{payload.Email}_{Guid.NewGuid().ToString("N").Substring(0, 8)}", // Unique username
                    Email = payload.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Dummy hash
                    Role = RoleEnum.Parent,
                    Status = "Active",
                    CreatedBy = "System",
                    LastUpdatedBy = "System"
                };
                await _accountRepository.AddUserAsync(user);
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

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrEmpty(usernameOrEmail))
                throw new ArgumentNullException(nameof(usernameOrEmail), "Username or email cannot be null or empty.");

            return await _accountRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
        }

    }
}
