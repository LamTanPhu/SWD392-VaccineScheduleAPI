using BCrypt.Net;
using Core.Utils;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Requests.Forgot_Password;
using ModelViews.Responses.Auth;
using ModelViews.Responses.Forgot_Password;
using Repositories.Repository;
using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Services.Services.Accounts
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SmtpSettings _smtpSettings;

        public AuthService(
            IAccountRepository accountRepository,
            IJwtService jwtService,
            IFirebaseAuthService firebaseAuthService,
            IUnitOfWork unitOfWork,
            EmailSettings emailSettings)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _firebaseAuthService = firebaseAuthService ?? throw new ArgumentNullException(nameof(firebaseAuthService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _smtpSettings = _smtpSettings ?? throw new ArgumentNullException(nameof(_smtpSettings));
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

        private async Task SendResetEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(_smtpSettings.Host) || string.IsNullOrEmpty(_smtpSettings.Username) || string.IsNullOrEmpty(_smtpSettings.Password))
                throw new InvalidOperationException("SMTP settings are not properly configured.");

            var verifyLink = $"http://localhost:5184/api/Authentication/verify-reset?token={token}";
            var smtpClient = new SmtpClient(_smtpSettings.Host)
            {
                Port = _smtpSettings.Port,
                Credentials = new System.Net.NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = "Reset Your Password",
                Body = $"Click the link to verify and reset your password: <a href='{verifyLink}'>{verifyLink}</a>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }


        public async Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return new ForgotPasswordResponseDTO { Success = false, Message = "Email is required." };

            var account = await _accountRepository.GetByEmailAsync(request.Email);
            if (account == null || account.DeletedTime != null)
                return new ForgotPasswordResponseDTO { Success = false, Message = "Email not found or account deleted." };

            var resetToken = _jwtService.GenerateShortLivedJwtToken(account); // Truyền Account thay vì Claim[]

            await SendResetEmail(account.Email, resetToken);

            return new ForgotPasswordResponseDTO { Success = true, Message = "Reset password link sent to your email." };
        }

        public async Task<VerifyResetResponseDTO> VerifyResetAsync(VerifyResetRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return new VerifyResetResponseDTO { Success = false, Message = "Token is required." };

            try
            {
                var principal = _jwtService.ValidateJwtToken(request.Token);
                if (principal == null || _jwtService.IsTokenExpired(request.Token))
                    return new VerifyResetResponseDTO { Success = false, Message = "Invalid or expired token." };

                var accountId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                    return new VerifyResetResponseDTO { Success = false, Message = "Invalid token: AccountId not found." };

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null || account.DeletedTime != null)
                    return new VerifyResetResponseDTO { Success = false, Message = "Account not found or deleted." };

                return new VerifyResetResponseDTO
                {
                    Success = true,
                    Message = "Verification successful. Proceed to reset password.",
                    AccountId = accountId
                };
            }
            catch (Exception ex)
            {
                return new VerifyResetResponseDTO { Success = false, Message = $"Token validation failed: {ex.Message}" };
            }
        }


    }
}