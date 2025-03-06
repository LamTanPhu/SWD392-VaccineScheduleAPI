using IRepositories.Entity.Schedules;
using IRepositories.IRepository.Schedules;
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
    }
}
