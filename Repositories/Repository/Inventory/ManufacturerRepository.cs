using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository.Inventory;

namespace Repositories.Repository.Inventory
{
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(DatabaseContext context) : base(context) { }

        // Custom method to get Manufacturer by Name
        public async Task<Manufacturer?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name.Equals(name));
        }
    }
}
