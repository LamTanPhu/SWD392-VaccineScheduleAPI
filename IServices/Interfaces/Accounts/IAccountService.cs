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
        Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken);  // New method for Google login
        Task<Account?> GetUserByEmailAsync(string email); 
        Task UpdateUserAsync(Account user);
        Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<IEnumerable<AccountResponseDTO>> GetAllAccountsAsync(); // Cập nhật trả về DTO
        Task<AccountResponseDTO?> GetAccountByIdAsync(string id); // Thêm Get by ID
        Task<bool> SoftDeleteAccountAsync(string id); // Thêm Soft Delete

    }
}
