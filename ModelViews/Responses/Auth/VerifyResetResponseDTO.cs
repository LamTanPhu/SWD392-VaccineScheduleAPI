using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Auth
{
    public class VerifyResetResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccountId { get; set; } // Trả về AccountId để dùng ở bước reset
    }
}
