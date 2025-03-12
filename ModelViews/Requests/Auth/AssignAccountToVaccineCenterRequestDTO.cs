using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Auth
{
    public class AssignAccountToVaccineCenterRequestDTO
    {
        public string AccountId { get; set; }
        public string VaccineCenterId { get; set; }
    }
}
