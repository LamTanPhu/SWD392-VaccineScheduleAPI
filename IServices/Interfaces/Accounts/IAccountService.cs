using IRepositories.Entity.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAccountService
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);  // Added LoginAsync method
        Task<Account?> GetUserByEmailAsync(string email); // Add this method
        Task UpdateUserAsync(Account user); // Add this method
        Task<string> HashPassword(string password); // Add this method
    }
}
