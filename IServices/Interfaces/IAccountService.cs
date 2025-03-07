using IRepositories.Entity;
using IRepositories.Entity.Accounts;
using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> GetByUsernameAsync(string username);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
    }
}
