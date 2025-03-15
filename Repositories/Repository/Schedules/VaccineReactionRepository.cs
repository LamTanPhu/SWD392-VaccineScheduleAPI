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
    public class VaccineReactionRepository : GenericRepository<VaccineReaction>, IVaccineReactionRepository
    {
        public VaccineReactionRepository(DatabaseContext context) : base(context) { }
    }
}
