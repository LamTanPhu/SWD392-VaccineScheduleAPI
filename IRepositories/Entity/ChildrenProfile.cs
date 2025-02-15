using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Core.Base;

namespace IRepositories.Entity
{

    public class ChildrenProfile : BaseEntity
    {
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }

        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; }
    }

}
