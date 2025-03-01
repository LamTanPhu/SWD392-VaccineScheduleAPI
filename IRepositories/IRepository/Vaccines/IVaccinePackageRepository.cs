using IRepositories.Entity.Vaccines;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Vaccines
{
    public interface IVaccinePackageRepository : IGenericRepository<VaccinePackage>
    {
        Task<IEnumerable<VaccinePackage>> GetActivePackagesAsync();
    }
}
