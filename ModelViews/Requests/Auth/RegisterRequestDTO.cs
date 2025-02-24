using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepositories.Enum;  // Import RoleEnum

namespace ModelViews.Requests.Auth
{
    public class RegisterRequestDTO
    {
        public string Email { get; set; }
        public string Username { get; set; }  // ✅ Added Username field
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public RoleEnum Role { get; set; }  // ✅ Use Enum for role
    }
}
