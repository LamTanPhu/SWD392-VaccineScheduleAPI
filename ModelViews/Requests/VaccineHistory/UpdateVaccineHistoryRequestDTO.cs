using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineHistory
{
    public class UpdateVaccineHistoryRequestDTO
    {
        public DateTime AdministeredDate { get; set; }
        public string AdministeredBy { get; set; }
        public bool DocumentationProvided { get; set; }
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
        public int VaccinedStatus { get; set; }
        public int DosedNumber { get; set; }
    }
}
