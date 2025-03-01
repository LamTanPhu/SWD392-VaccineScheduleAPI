using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using IRepositories.IRepository;
using IServices.Interfaces.Accounts;
using IRepositories.Entity.Accounts;

namespace Services.Services.Accounts
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account), "Account cannot be null");

            if (string.IsNullOrEmpty(account.Username))
                throw new ArgumentNullException(nameof(account.Username), "Username cannot be null or empty");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role.ToString())  // Adding the Role claim
            };

            // Retrieve the secret key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Using dynamic config for issuer
                audience: _configuration["Jwt:Audience"], // Using dynamic config for audience
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Account> ExtractAccountAsync(string token)
        {
            var claims = ExtractAllClaims(token);
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // You might want to load account data based on username from the database
            // For simplicity, we assume it's retrieved from the database
            var account = new Account
            {
                Username = username,
                // Add other fields as needed (e.g., Email, Role)
            };

            return account;
        }

        public bool IsTokenExpired(string token)
        {
            var expiration = ExtractExpiration(token);
            return expiration < DateTime.Now;
        }

        public DateTime ExtractExpiration(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtToken?.ValidTo ?? DateTime.MinValue;
        }

        public IEnumerable<Claim> ExtractAllClaims(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtToken?.Claims ?? Enumerable.Empty<Claim>();
        }
    }
}
