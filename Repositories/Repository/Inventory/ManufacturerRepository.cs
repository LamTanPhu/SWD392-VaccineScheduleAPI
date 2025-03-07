using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using IRepositories.Entity.Orders;
using IRepositories.IRepository.Orders;
using IRepositories.Entity.Inventory;
using IRepositories.IRepository.Inventory;

namespace Repositories.Repository.Orders
{
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(DatabaseContext context) : base(context) { }

       
        //Custom method to get Manufacturer by Name
        public async Task<Manufacturer?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name.Equals(name));
        }
    }
}