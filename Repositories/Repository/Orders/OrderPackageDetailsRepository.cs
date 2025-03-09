using IRepositories.Entity.Orders;
using IRepositories.IRepository.Orders;
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
    }
}
