using IRepositories.Entity.Accounts;
using IRepositories.IRepository;
using IRepositories.IRepository.Accounts;
using Repositories.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository.Accounts
{
    public class ChildrenProfileRepository : GenericRepository<ChildrenProfile>, IChildrenProfileRepository
    {
        public ChildrenProfileRepository(DatabaseContext context) : base(context) { }
    }
}
