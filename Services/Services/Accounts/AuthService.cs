using BCrypt.Net;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IFirebaseAuthService _firebaseAuthService;

        public AuthService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IFirebaseAuthService firebaseAuthService)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _firebaseAuthService = firebaseAuthService ?? throw new ArgumentNullException(nameof(firebaseAuthService));
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Login request cannot be null.");

            if (string.IsNullOrEmpty(request.UsernameOrEmail) || string.IsNullOrEmpty(request.Password))
                return new LoginResponseDTO
                {
                    id=null,
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
                    id = null,
                    Username = null,
                    Role = RoleEnum.Parent,
                    Token = string.Empty,
                    Expiration = DateTime.MinValue
                };
            }

            var token = _jwtService.GenerateJwtToken(user);
            var expiration = _jwtService.ExtractExpiration(token);

            return new LoginResponseDTO
            {
                id=user.Id,
                Username = user.Username, // Nullable, as per Account model
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
    }
}