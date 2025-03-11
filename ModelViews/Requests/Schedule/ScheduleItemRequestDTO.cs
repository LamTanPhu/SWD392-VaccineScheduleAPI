using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Schedule
{
    public class ScheduleItemRequestDTO
    {
        public string? OrderVaccineDetailsId { get; set; } // For individual vaccines
        public string? OrderPackageDetailsId { get; set; } // For vaccine packages
        public int DoseNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string VaccineCenterId { get; set; }
    }
}
