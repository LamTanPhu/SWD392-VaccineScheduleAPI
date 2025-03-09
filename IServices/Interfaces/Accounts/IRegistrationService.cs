using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IRegistrationService
    {
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
    }
}