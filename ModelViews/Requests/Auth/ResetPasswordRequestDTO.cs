using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Auth
{
    public class ResetPasswordRequestDTO
    {
        public string AccountId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}