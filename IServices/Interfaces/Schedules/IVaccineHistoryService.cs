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
        Task<VaccineHistoryResponseDTO> CreateVaccineHistoryAsync(CreateVaccineHistoryRequestDTO request, string accountId);

    }
}