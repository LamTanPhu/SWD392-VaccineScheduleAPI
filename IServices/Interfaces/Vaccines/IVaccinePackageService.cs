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
        Task AddPackageAsync(VaccinePackageRequestDTO package);
        Task UpdatePackageAsync(string id, VaccinePackageRequestDTO packageDto);
        Task DeletePackageAsync(string id);
    }
}