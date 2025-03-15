using BCrypt.Net;
using IRepositories.Entity.Accounts;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IAccountRepository _accountRepository;

        public RegistrationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
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
                PhoneNumber = "Not Update",
                ImageProfile = "Not Update ",
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
    }
}