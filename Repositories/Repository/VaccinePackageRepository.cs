using IRepositories.Entity;
using IRepositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class VaccinePackageRepository : GenericRepository<VaccinePackage>, IVaccinePackageRepository
    {
        public VaccinePackageRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<VaccinePackage>> GetActivePackagesAsync()
        {
            return await _dbSet.Where(p => p.PackageStatus == true).ToListAsync();
        }
    }
}