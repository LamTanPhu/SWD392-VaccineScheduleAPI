using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Principal;
using Core.Base;


namespace IRepositories.Entity
{

    public class VaccineCenter : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }


        // Navigation properties
        public virtual ICollection<VaccineBatch> VaccineBatches { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; }
    }


}
