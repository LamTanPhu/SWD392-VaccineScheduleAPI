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
        Task<ClaimsPrincipal> ValidateTokenAsync(string token); // Return claims instead of Account
        bool IsTokenExpired(string token);
        DateTime ExtractExpiration(string token);
        IEnumerable<Claim> ExtractAllClaims(string token);

        /////
        string GenerateShortLivedJwtToken(Account account);
        ClaimsPrincipal ValidateJwtToken(string token);
    }
}
