using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.History
{
    public class VaccineHistoryRequestDTO
    {
        public string VaccineId { get; set; }
        public string ProfileId { get; set; }
        public string AccountId { get; set; }
        public string CenterId { get; set; }
        public DateTime AdministeredDate { get; set; }
        public string AdministeredBy { get; set; }
        public string DocumentationProvided { get; set; }
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
        public int VaccinedStatus { get; set; }
        public int DosedNumber { get; set; }
    }

}
