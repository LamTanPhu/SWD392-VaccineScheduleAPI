using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Auth
{
    public class AssignAccountToVaccineCenterResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ProfileResponseDTO Account { get; set; }
    }
}
