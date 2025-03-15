using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken);

        Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request);
    }
}