using ModelViews.Requests.Auth;
using ModelViews.Requests.Forgot_Password;
using ModelViews.Responses.Auth;
using ModelViews.Responses.Forgot_Password;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<LoginResponseDTO> LoginWithGoogleAsync(string idToken);

        Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request);
        Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
        Task<VerifyResetResponseDTO> VerifyResetAsync(VerifyResetRequestDTO request);


    }
}