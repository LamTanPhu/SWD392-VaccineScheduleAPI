using ModelViews.Requests.VaccineCenter;
using ModelViews.Responses.VaccineCenter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IServices.Interfaces
{
    public interface IVaccineCenterService
    {
        Task<IList<VaccineCenterResponseDTO>> GetAllAsync(); // Read all
        Task<VaccineCenterResponseDTO?> GetByIdAsync(string id); // Read by ID
        Task<VaccineCenterResponseDTO> AddAsync(VaccineCenterRequestDTO model);

        Task UpdateAsync(VaccineCenterUpdateDTO center); // Update
        Task DeleteAsync(VaccineCenterDeleteDTO center); // Delete
        Task<IList<VaccineCenterResponseDTO>> GetByNameAsync(string name);
    }
}
