using ModelViews.Requests.VaccineCategory;
using ModelViews.Responses.VaccineCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Vaccines
{
    public interface IVaccineCategoryService
    {
        Task<IEnumerable<VaccineCategoryResponseDTO>> GetAllCategoriesAsync();
        Task<VaccineCategoryResponseDTO?> GetCategoryByIdAsync(string id);
        Task AddCategoryAsync(VaccineCategoryRequestDTO category);
        Task UpdateCategoryAsync(string id, VaccineCategoryRequestDTO categoryDto);
        Task DeleteCategoryAsync(string id);
    }
}
