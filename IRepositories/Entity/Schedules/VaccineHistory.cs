using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Inventory;
using IRepositories.Entity.Vaccines;

namespace IRepositories.Entity.Schedules
{

    public class VaccineHistory : BaseEntity
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


        public virtual Vaccine Vaccine { get; set; }
        public virtual ChildrenProfile Profile { get; set; }
        public virtual Account Account { get; set; }
        public virtual VaccineCenter Center { get; set; }
    }

}
