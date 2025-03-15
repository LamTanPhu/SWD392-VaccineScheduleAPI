using IRepositories.IRepository.Orders;
using Repositories.Context;
using IRepositories.Entity.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repository.Orders
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext context) : base(context) { }


    }
}