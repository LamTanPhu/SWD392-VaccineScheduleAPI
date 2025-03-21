using IRepositories.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ModelViews.Responses.Auth
{
    public class AccountResponseDTO
    {
        public string VaccineCenterId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageProfile { get; set; }
        public string Status {  get; set; }

    }
}
