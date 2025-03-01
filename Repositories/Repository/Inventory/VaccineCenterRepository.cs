using Core.Base;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository.Inventory;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace Repositories.Repository.Inventory
{
    public class VaccineCenterRepository : GenericRepository<VaccineCenter>, IVaccineCenterRepository
    {
        public VaccineCenterRepository(DatabaseContext context) : base(context) { }

        public async Task<VaccineCenter?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(vc => vc.Name == name);
        }
    }
}
