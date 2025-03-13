using IRepositories.IRepository.Orders;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IRepositories.IRepository;
using Core;
using IRepositories.Entity.Orders;

namespace Repositories.Repository.Orders
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext context) : base(context) { }

        IQueryable<IRepositories.Entity.Orders.Order> IGenericRepository<IRepositories.Entity.Orders.Order>.Entities => throw new NotImplementedException();

        public Task<BasePaginatedList<IRepositories.Entity.Orders.Order>> GetPagging(IQueryable<IRepositories.Entity.Orders.Order> query, int index, int pageSize)
        {
            throw new NotImplementedException();
        }

        public void Insert(IRepositories.Entity.Orders.Order obj)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(IRepositories.Entity.Orders.Order obj)
        {
            throw new NotImplementedException();
        }

        public void InsertRange(IList<IRepositories.Entity.Orders.Order> obj)
        {
            throw new NotImplementedException();
        }

        public void Update(IRepositories.Entity.Orders.Order obj)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IRepositories.Entity.Orders.Order obj)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IRepositories.Entity.Orders.Order> IGenericRepository<IRepositories.Entity.Orders.Order>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<IList<IRepositories.Entity.Orders.Order>> IGenericRepository<IRepositories.Entity.Orders.Order>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        IRepositories.Entity.Orders.Order? IGenericRepository<IRepositories.Entity.Orders.Order>.GetById(object id)
        {
            throw new NotImplementedException();
        }

        Task<IRepositories.Entity.Orders.Order?> IGenericRepository<IRepositories.Entity.Orders.Order>.GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
    }
}

