using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Auth
{
    public class UpdateAccountRequestDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string? VaccineCenterId { get; set; } // Nullable to allow clearing the assignment
    }
}