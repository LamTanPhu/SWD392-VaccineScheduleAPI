using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
using Microsoft.EntityFrameworkCore;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Schedules
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<Feedback>> GetByOrderIdAsync(string orderId)
        {
            return (IEnumerable<Feedback>)await _dbSet.FirstOrDefaultAsync(m => m.OrderId.Equals(orderId));
        }
    }
}
