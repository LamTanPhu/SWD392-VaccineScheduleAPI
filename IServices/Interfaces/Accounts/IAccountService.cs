using IRepositories.Entity.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Requests.Mail;
using ModelViews.Responses.Auth;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAccountService
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(Account user);
        string HashPassword(string password); // Không cần async vì là hàm đồng bộ

        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
        Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request);
    }
}
