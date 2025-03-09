using IRepositories.Entity.Accounts;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using System;
using System.Threading.Tasks;

namespace Services.Services.Accounts
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAccountRepository _accountRepository;

        public UserProfileService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");

            return await _accountRepository.GetByUsernameAsync(username);
        }

        public async Task<Account?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");

            return await _accountRepository.GetByEmailAsync(email);
        }

        public async Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrEmpty(usernameOrEmail))
                throw new ArgumentNullException(nameof(usernameOrEmail), "Username or email cannot be null or empty.");

            return await _accountRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
        }

        public async Task UpdateUserAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            await _accountRepository.UpdateUserAsync(user);
        }
    }
}