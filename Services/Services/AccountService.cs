using Core.Utils;
using IRepositories.Entity;
using IRepositories.IRepository;
using IServices.Interfaces;
using ModelViews.DTOs;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _accountRepository.GetByUsernameAsync(username);
        }
        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO model)
        {
            var existingUser = await _accountRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new RegisterResponseDTO { Success = false, Message = "Email is already registered." };
            }

            var user = new Account
            {
                Email = model.Email,
                PasswordHash = model.Password,
                Role = "User",
                Status = "Active"
            };

            var success = await _accountRepository.AddUserAsync(user);
            return new RegisterResponseDTO { Success = success, Message = success ? "User registered successfully." : "Registration failed." };
        }
    }
}
