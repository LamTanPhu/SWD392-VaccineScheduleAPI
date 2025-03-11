using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Schedule
{
    public class ScheduleResponseDTO
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string VaccineCenterId { get; set; }
        public string? OrderVaccineDetailsId { get; set; }
        public string? OrderPackageDetailsId { get; set; }
        public int DoseNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? AdministeredBy { get; set; }
        public int Status { get; set; }
    }
}
