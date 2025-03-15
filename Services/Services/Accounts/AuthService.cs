using BCrypt.Net;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using Repositories.Repository;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IFirebaseAuthService firebaseAuthService,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _firebaseAuthService = firebaseAuthService ?? throw new ArgumentNullException(nameof(firebaseAuthService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        public async Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            if (request == null)
                return new ResetPasswordResponseDTO { Success = false, Message = "Request cannot be null." };

            if (string.IsNullOrEmpty(request.AccountId) || string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
                return new ResetPasswordResponseDTO { Success = false, Message = "All fields are required." };

            if (request.NewPassword != request.ConfirmPassword)
                return new ResetPasswordResponseDTO { Success = false, Message = "Passwords do not match." };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var account = await _accountRepository.GetByIdAsync(request.AccountId);
                if (account == null || account.DeletedTime != null)
                    return new ResetPasswordResponseDTO { Success = false, Message = "Account not found or has been deleted." };

                // Hash mật khẩu mới với BCrypt (tương tự RegistrationService)
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                // Cập nhật mật khẩu và thời gian chỉnh sửa
                account.PasswordHash = hashedPassword;
                account.LastUpdatedTime = DateTime.Now;
                account.LastUpdatedBy = "System"; // Có thể thay bằng user thực hiện reset nếu có context

                await _accountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new ResetPasswordResponseDTO
                {
                    Success = true,
                    Message = "Password reset successfully."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResetPasswordResponseDTO
                {
                    Success = false,
                    Message = $"Failed to reset password: {ex.Message}"
                };
            }
        }

    }
}