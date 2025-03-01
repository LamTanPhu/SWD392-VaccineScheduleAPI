using IRepositories.Entity.Vaccines;
using IRepositories.IRepository.Vaccines;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository.Vaccines
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