using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices.Interfaces.Inventory
{
    public interface IVaccineCenterService
    {
        Task<IList<VaccineCenterResponseDTO>> GetAllAsync();
        Task<IList<object>> GetAllPublicAsync();
        Task<VaccineCenterResponseDTO?> GetByIdAsync(string id);
        Task<VaccineCenterResponseDTO> AddAsync(VaccineCenterRequestDTO model);
        Task UpdateAsync(string id, VaccineCenterUpdateDTO model);
        Task DeleteAsync(string id); 
        Task<IList<VaccineCenterResponseDTO>> GetByNameAsync(string name);
    }
}
