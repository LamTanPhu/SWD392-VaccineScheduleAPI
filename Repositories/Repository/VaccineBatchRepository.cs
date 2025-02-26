using IRepositories.Entity;
using IRepositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class VaccineBatchRepository : GenericRepository<VaccineBatch>, IVaccineBatchRepository
    {
        public VaccineBatchRepository(DatabaseContext context) : base(context) { }

        public async Task<bool> AddBatchAsync(VaccineBatch batch)
        {
            await _dbSet.AddAsync(batch);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<VaccineBatch?> GetByBatchNumberAsync(string batchNumber)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(vb => vb.BatchNumber == batchNumber);
        }
    }
}
