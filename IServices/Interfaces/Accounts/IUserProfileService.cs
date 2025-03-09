using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IUserProfileService
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> GetUserByEmailAsync(string email);
        Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task UpdateUserAsync(Account user);
    }
}