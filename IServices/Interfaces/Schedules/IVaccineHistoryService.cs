using ModelViews.Requests.VaccineHistory;
using ModelViews.Responses.VaccineHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Schedules
{
    public interface IVaccineHistoryService
    {
        Task<CreateVaccineHistoryResponseDTO> AddVaccineHistoryAsync(AddVaccineHistoryRequestDTO request);
        Task<VaccineHistoryResponseDTO> GetVaccineHistoryByIdAsync(string id);
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync();
        Task<VaccineHistoryResponseDTO> UpdateVaccineHistoryAsync(string id, UpdateVaccineHistoryRequestDTO request);
        Task<bool> DeleteVaccineHistoryAsync(string id);
    }
}