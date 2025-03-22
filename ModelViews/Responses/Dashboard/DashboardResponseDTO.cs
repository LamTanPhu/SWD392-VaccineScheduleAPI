using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Dashboard
{
    public class DashboardResponseDTO
    {
        public int TotalAccounts { get; set; }
        public int AdminAccounts { get; set; }
        public int StaffAccounts { get; set; }
        public int ParentAccounts { get; set; }
        public int TotalChildrenProfile { get; set; }
        public int TotalVaccineCenters { get; set; }
        public int TotalVaccinesAvailable { get; set; }
        public int TotalOrdersPaid { get; set; }
        public int TotalSchedule { get; set; }
    }
}
