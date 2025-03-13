using IRepositories.Entity.Orders;
using IRepositories.IRepository.Orders;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Orders
{
    public class OrderPackageDetailsRepository : GenericRepository<OrderPackageDetails>, IOrderPackageDetailsRepository
    {
        public OrderPackageDetailsRepository(DatabaseContext context) : base(context) { }
        public async Task<OrderPackageDetails> GetByIdWithPackageDetailsAsync(string id)
        {
            return await _context.OrderPackageDetails
                .Include(pd => pd.VaccinePackage)
                .ThenInclude(vp => vp.PackageDetails)
                .FirstOrDefaultAsync(pd => pd.Id == id);
        }
    }
}
