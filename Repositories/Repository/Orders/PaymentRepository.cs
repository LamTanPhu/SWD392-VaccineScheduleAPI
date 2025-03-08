using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Orders;
using IRepositories.IRepository.Orders;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;

namespace Repositories.Repository.Orders
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(DatabaseContext context) : base(context) { }

        public async Task<bool> AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Payment?> GetByPaymentnameAsync(string name)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.PaymentName == name);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
