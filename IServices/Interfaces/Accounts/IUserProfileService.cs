using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Entity.Accounts;
using System.Threading.Tasks;
using ModelViews.Responses.Auth;

namespace IServices.Interfaces.Accounts
{
    public interface IUserProfileService
    {
        Task<ProfileResponseDTO?> GetProfileByEmailAsync(string email);

        Task<ProfileResponseDTO?> GetProfileByUsernameAsync(string username);
        Task<Account?> GetByUsernameAsync(string username);
        Task<Account?> GetUserByEmailAsync(string email);   
        Task<Account?> GetByEmailAsync(string email);       
        Task<Account?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task AddUserAsync(Account user);
        Task UpdateUserAsync(Account user);
    }
}