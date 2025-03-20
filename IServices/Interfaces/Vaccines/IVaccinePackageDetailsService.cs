using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccinePackageDetailsService
    {
        Task<IList<VaccinePackageDetailsResponseDTO>> GetAllDetailsAsync();
        Task<VaccinePackageDetailsResponseDTO> GetDetailByIdAsync(string id);
        Task<VaccinePackageDetailsResponseDTO> AddDetailAsync(VaccinePackageDetailsRequestDTO detailDto);
        Task UpdateDetailAsync(string id, VaccinePackageDetailsRequestDTO detailDto);
        Task DeleteDetailAsync(string id);
    }
}
