using IRepositories.Entity.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Schedules
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {

      Task<IEnumerable<Feedback>> GetByOrderIdAsync(string orderId);

    }
}
