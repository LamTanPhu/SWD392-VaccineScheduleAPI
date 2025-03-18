using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Forgot_Password
{
    public class VerifyResetResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccountId { get; set; }
    }
}
