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
        Task<VaccineBatch?> GetByBatchNumberAsync(string batchNumber);
        Task<AddVaccineBatchResponseDTO> AddBatchAsync(AddVaccineBatchRequestDTO request);
        Task<IEnumerable<VaccineBatchResponseDTO>> SearchByNameAsync(string name);
    }
}
