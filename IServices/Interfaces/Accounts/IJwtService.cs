using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using IRepositories.Entity.Accounts;

namespace IServices.Interfaces.Accounts
{
    public interface IJwtService
    {
        string GenerateJwtToken(Account account);
        Task<Account> ExtractAccountAsync(string token);
        bool IsTokenExpired(string token);
        DateTime ExtractExpiration(string token);
        IEnumerable<Claim> ExtractAllClaims(string token);  // Changed to return IEnumerable<Claim>
    }
}
