using IRepositories.Entity;
using IRepositories.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRepositories.IRepository
{
    public interface IVaccinePackageRepository : IGenericRepository<VaccinePackage>
    {
        Task<IEnumerable<VaccinePackage>> GetActivePackagesAsync();
    }
}
