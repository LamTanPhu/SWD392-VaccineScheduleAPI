using IRepositories.Entity;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccinePackageService
    {
Task<IEnumerable<VaccinePackageResponseDTO>> GetAllPackagesAsync();
        Task<VaccinePackageResponseDTO?> GetPackageByIdAsync(string id);
        Task<VaccinePackageResponseDTO> AddPackageAsync(VaccinePackageRequestDTO package);
        Task<VaccinePackageResponseDTO?> UpdatePackageAsync(string id, VaccinePackageRequestDTO packageDto);
        Task AddVaccineToPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request);
        Task RemoveVaccineFromPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request);
        Task DeletePackageAsync(string id);
        Task<CombinedVaccineResponseDTO> GetAllVaccinesAndPackagesAsync(); // Giữ lại nhưng có thể bỏ nếu không cần
    }
}