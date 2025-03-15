using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAccountAssignmentService
    {
        Task<AssignAccountToVaccineCenterResponseDTO> AssignAccountToVaccineCenterAsync(AssignAccountToVaccineCenterRequestDTO request);
    }
}