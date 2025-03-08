using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Context;
using Microsoft.EntityFrameworkCore;
using ModelViews.DTOs;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using IRepositories.IRepository.Accounts;

namespace Repositories.Repository.Accounts
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(DatabaseContext context) : base(context) { }

        public async Task<bool> AddUserAsync(Account user)
        {
            await _dbSet.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateUserAsync(Account user)
        {
            _dbSet.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail);
        }

    }
}
