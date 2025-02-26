using IRepositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository
{
    public interface IVaccineBatchRepository : IGenericRepository<VaccineBatch>
    {
        Task<VaccineBatch?> GetByBatchNumberAsync(string batchNumber);
        Task<bool> AddBatchAsync(VaccineBatch batch);
    }
}
