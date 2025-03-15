using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Schedules
{
    public class VaccineHistoryRepository : GenericRepository<VaccineHistory>, IVaccineHistoryRepository
    {
        public VaccineHistoryRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<VaccineHistory>> GetByUserIdAsync(string userId)
        {
            return await _dbSet.Where(vh => vh.AccountId == userId).ToListAsync();
        }

        public async Task<IEnumerable<VaccineHistory>> SearchByDateAsync(string userId, DateTime date)
        {
            return await _dbSet.Where(vh => vh.AccountId == userId && vh.AdministeredDate.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<VaccineHistory>> SearchByCenterIdAsync(string centerId)
        {
            return await _dbSet.Where(vh => vh.CenterId == centerId).ToListAsync();
        }
    }
}
