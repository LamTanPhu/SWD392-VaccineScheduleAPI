using IRepositories.Entity.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Orders
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
