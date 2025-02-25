using IRepositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository
{
    public interface IManufacturerRepository : IGenericRepository<Manufacturer>
    {
        Task<Manufacturer?> GetByNameAsync(string name);

    }
}
