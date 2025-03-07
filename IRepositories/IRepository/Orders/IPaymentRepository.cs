using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Orders
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByPaymentnameAsync(string name);
        Task<bool> AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
    }
}
