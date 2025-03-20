using IRepositories.Entity.Inventory;
using ModelViews.Requests.VaccineBatch;
using ModelViews.Responses.VaccineBatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Inventory
{
    public interface IVaccineBatchService
    {
        Task<IEnumerable<VaccineBatchResponseDTO>> GetAllAsync();
        Task<VaccineBatchResponseDTO?> GetByBatchNumberAsync(string batchNumber);
        Task<IEnumerable<VaccineBatchResponseDTO>> SearchByNameAsync(string name);
        Task<VaccineBatchResponseDTO> CreateAsync(AddVaccineBatchRequestDTO request);
        Task DeleteAsync(string id);
    }
}
