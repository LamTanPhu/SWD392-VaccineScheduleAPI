using IRepositories.Entity;
using IRepositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Context;
using Microsoft.EntityFrameworkCore;
using ModelViews.DTOs;
namespace Repositories.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DatabaseContext _context;

        public AccountRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserAsync(Account user)
        {
            await _context.Accounts.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Account> GetByUsernameAsync(string username)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }
   
    }
}
