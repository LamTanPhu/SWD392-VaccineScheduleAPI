using IRepositories.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Auth
{
    public class RegisterResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? UserId { get; set; }  // ✅ Optional: Return User ID if needed
        public RoleEnum Role { get; set; }  // ✅ Include Role in response
    }
}
