using IRepositories.Entity.Accounts;
using IRepositories.Enum;
using IRepositories.IRepository.Accounts;
using IServices.Interfaces.Accounts;
using ModelViews.Responses.Auth;
using ModelViews.Responses.ChildrenProfile;
using ModelViews.Responses.VaccineCenter;
using System;
using System.Linq;
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

        public async Task<ProfileResponseDTO?> GetProfileByUsernameAsync(string username)
        {
            var account = await _accountRepository.GetByUsernameAsync(username);
            if (account == null || account.DeletedTime != null)
            {
                Console.WriteLine($"Account null or deleted for username: {username}");
                return null;
            }

            var dto = MapToProfileResponseDTO(account);
            Console.WriteLine($"Mapped DTO: Id(not included)={account.Id}, Username={dto.Username}, Email={dto.Email}, Status={dto.Status}, Role={dto.Role}");
            return dto;
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

        public async Task<Account?> GetByEmailAsync(string email)
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

        public async Task AddUserAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            await _accountRepository.InsertAsync(user);
            await _accountRepository.SaveAsync();
        }

        public async Task UpdateUserAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            await _accountRepository.UpdateAsync(user);
        }

        private ProfileResponseDTO MapToProfileResponseDTO(Account account)
        {
            return new ProfileResponseDTO
            {
                Username = account.Username,
                Email = account.Email,
                Role = account.Role.ToString(),
                Status = account.Status,
                VaccineCenter = account.VaccineCenter != null ? new VaccineCenterResponseDTO
                {
                    Id = account.VaccineCenter.Id,
                    Name = account.VaccineCenter.Name,
                    Location = account.VaccineCenter.Location,
                    ContactNumber = account.VaccineCenter.ContactNumber,
                    Email = account.VaccineCenter.Email,
                    Status = account.VaccineCenter.Status
                } : null,
                ChildrenProfiles = account.ChildrenProfiles?.Any() == true ? account.ChildrenProfiles.Select(cp => new ChildrenProfileResponseDTO
                {
                    Id = cp.Id,
                    AccountId = cp.AccountId,
                    FullName = cp.FullName,
                    DateOfBirth = cp.DateOfBirth,
                    Gender = cp.Gender,
                    Status = cp.Status
                }).ToList() : null
            };
        }
    }
}