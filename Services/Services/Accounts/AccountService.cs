using BCrypt.Net;
using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using IServices.Interfaces.Mail;
using ModelViews.Requests.Auth;
using ModelViews.Requests.Mail;
using ModelViews.Responses.Auth;
using Services.Services.Mail;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
         // New dependency for Firebase token validation
        private readonly IEmailService _emailService;

        public AccountService(IAccountRepository accountRepository, IEmailService emailService, IJwtService jwtService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
    
            _emailService = emailService;

        }

        public async Task<Account?> GetUserByEmailAsync(string email)
            => await _accountRepository.GetByEmailAsync(email);

        public async Task UpdateUserAsync(Account user)
            => await _accountRepository.UpdateUserAsync(user);

        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public async Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            var user = await _accountRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return new ForgotPasswordResponseDTO { Success = false, Message = "User not found." };

            // Generate OTP and expiry time
            var otp = new Random().Next(100000, 999999).ToString();
            user.OTP = otp;
            user.OTPExpired = DateTime.UtcNow.AddMinutes(5);

            await _accountRepository.UpdateUserAsync(user);

            // Send OTP via email
            var mailRequest = new Mailrequest
            {
                ToEmail = request.Email,
                Subject = "Password Reset OTP",
                Body = $"Your OTP for password reset is: {otp} (Valid for 5 minutes)."
            };
            await _emailService.SendEmailAsync(mailRequest);

            return new ForgotPasswordResponseDTO { Success = true, Message = "OTP has been sent to your email." };
        }

        public async Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            var user = await _accountRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return new ResetPasswordResponseDTO { Success = false, Message = "User not found." };

            // Validate OTP
            if (user.OTP != request.Otp || user.OTPExpired == null || user.OTPExpired < DateTime.UtcNow)
                return new ResetPasswordResponseDTO { Success = false, Message = "Invalid or expired OTP." };

            // Reset password
            user.PasswordHash = HashPassword(request.NewPassword);
            user.OTP = null;
            user.OTPExpired = null;

            await _accountRepository.UpdateUserAsync(user);

            return new ResetPasswordResponseDTO { Success = true, Message = "Password has been reset successfully." };
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
            var user = await _accountRepository.GetByUsernameOrEmailAsync(request.Username);
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
                Role = user.Role,
                Token = token,
                Expiration = expiration
            };
        }

        public Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            throw new NotImplementedException();
        }
    }
}
