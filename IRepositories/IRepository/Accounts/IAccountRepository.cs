using IRepositories.Entity.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositories.IRepository.Accounts
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account> GetByEmailAsync(string email);
        Task<bool> AddUserAsync(Account user);
    }
}

