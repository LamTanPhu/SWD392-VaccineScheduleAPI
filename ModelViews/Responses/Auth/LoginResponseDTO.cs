using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using IRepositories.Enum;

namespace ModelViews.Responses.Auth
{
    public class LoginResponseDTO
    {
        public string id { get; set; }
        public string Username { get; set; }
        public RoleEnum Role { get; set; }  // ✅ Use Enum instead of string
        public string Token { get; set; }  // ✅ JWT Token
        public DateTime Expiration { get; set; }  // ✅ Token Expiration Time
    }
}
