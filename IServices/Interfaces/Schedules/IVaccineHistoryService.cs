using ModelViews.Requests.History;
using ModelViews.Responses.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Schedules
{
    public interface IVaccineHistoryService
    {
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllAsync();
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<VaccineHistoryResponseDTO>> SearchByDateAsync(string userId, DateTime date);
        Task<IEnumerable<VaccineHistoryResponseDTO>> SearchByCenterIdAsync(string centerId);
        Task CreateAsync(VaccineHistoryRequestDTO requestDto);
        Task UpdateAsync(string id, VaccineHistoryRequestDTO requestDto);
    }
}
