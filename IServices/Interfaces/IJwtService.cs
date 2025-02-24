using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Entity;
using System.Security.Claims;

namespace IServices.Interfaces
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
