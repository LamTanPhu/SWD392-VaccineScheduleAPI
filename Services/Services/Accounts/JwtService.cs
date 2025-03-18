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
        //Forgot Password
        private readonly byte[] _key;
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-with-at-least-16-chars");
        }

        public string GenerateJwtToken(Account account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrEmpty(account.Username)) throw new ArgumentNullException(nameof(account.Username));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.NameIdentifier, account.Id) // Changed from ClaimTypes.Email to ClaimTypes.NameIdentifier
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Generated Token: '{tokenString}'"); // Log generated token
            return tokenString;
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var principal = await Task.Run(() =>
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    }, out _));
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Console.WriteLine($"Token expired: {ex.Message}");
                return null;
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Console.WriteLine($"Invalid signature: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Validation failed: {ex.Message}");
                return null;
            }
        }

        public bool IsTokenExpired(string token) => ExtractExpiration(token) < DateTime.UtcNow; // Use UTC for consistency

        public DateTime ExtractExpiration(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                return jwtToken?.ValidTo ?? DateTime.MinValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to extract expiration: {ex.Message}");
                return DateTime.MinValue;
            }
        }

        public IEnumerable<Claim> ExtractAllClaims(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                return jwtToken?.Claims ?? Enumerable.Empty<Claim>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to extract claims: {ex.Message}");
                return Enumerable.Empty<Claim>();
            }
        }


        public string GenerateShortLivedJwtToken(Account account)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, account.Username ?? "reset-user"), // Thêm Name
        new Claim(ClaimTypes.Role, account.Role.ToString()),         // Thêm Role
        new Claim(ClaimTypes.Email, account.Email),                  // Thêm Email
        new Claim(ClaimTypes.NameIdentifier, account.Id)             // Thêm NameIdentifier (AccountId)
    };

            var key = new SymmetricSecurityKey(_key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds);
            var tokenString = _tokenHandler.WriteToken(token);
            Console.WriteLine($"Generated Short-Lived Token: '{tokenString}'"); // Log để kiểm tra
            return tokenString;
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var principal = _tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ClockSkew = TimeSpan.Zero
                }, out _);
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ValidateJwtToken failed: {ex.Message}");
                return null;
            }
        }



    }
}