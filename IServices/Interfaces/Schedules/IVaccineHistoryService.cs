using ModelViews.Requests.History;
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
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetAllVaccineHistoriesAsync();
        Task<VaccineHistoryResponseDTO?> GetVaccineHistoryByIdAsync(string id);
        Task<VaccineHistoryResponseDTO> AddVaccineHistoryAsync(CreateVaccineHistoryRequestDTO vaccineHistoryDto);
        Task<VaccineHistoryResponseDTO?> UpdateVaccineHistoryAsync(string id, CreateVaccineHistoryRequestDTO vaccineHistoryDto);
        Task<VaccineHistoryResponseDTO> SendVaccineCertificateAsync(SendVaccineCertificateRequestDTO certificateDto);
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetPendingCertificatesAsync();
        Task<VaccineHistoryResponseDTO?> VerifyCertificateAsync(string id, UpdateDocumentVaccineHistoryRequestDTO vaccineHistoryDto, bool isAccepted);
        Task<IEnumerable<VaccineHistoryResponseDTO>> GetVaccineHistoriesByChildIdAsync(string childId); // New method
    }
}