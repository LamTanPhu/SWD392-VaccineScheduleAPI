using ModelViews.Requests.Auth;
using ModelViews.Responses.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IAccountUpdateService
    {
        Task<ProfileResponseDTO> UpdateAccountAsync(string username, UpdateAccountRequestDTO request);
    }
}
