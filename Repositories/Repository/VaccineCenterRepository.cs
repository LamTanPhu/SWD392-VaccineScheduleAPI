using Core.Base;
using IRepositories.Entity;
using IRepositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace Repositories.Repository
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
