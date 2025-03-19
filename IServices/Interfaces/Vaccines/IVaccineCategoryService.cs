using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccineCategoryService
    {
        Task<IEnumerable<VaccineCategoryResponseDTO>> GetAllAsync();
        Task<VaccineCategoryResponseDTO?> GetByIdAsync(string id);
        Task<VaccineCategoryResponseDTO> CreateAsync(VaccineCategoryRequestDTO categoryDto);
        Task UpdateAsync(string id, VaccineCategoryRequestDTO categoryDto);
        Task DeleteAsync(string id);
    }
}