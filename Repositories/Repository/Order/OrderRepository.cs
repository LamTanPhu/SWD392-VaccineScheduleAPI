using IRepositories.IRepository.Orders;
using Repositories.Context;
using IRepositories.Entity.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Order
{
    public class OrderRepository : GenericRepository<IRepositories.Entity.Orders.Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext context) : base(context) { }
    }
}
