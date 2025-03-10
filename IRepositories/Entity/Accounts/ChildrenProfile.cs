using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using IRepositories.Entity.Orders;
using IRepositories.Entity.Schedules;

namespace IRepositories.Entity.Accounts
{

    public class ChildrenProfile : BaseEntity
    {
        // Foreign key to Account
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }

        public string Address { set; get; }

        // Navigation property
        public virtual Account Account { get; set; }  // EF will automatically use AccountId as the foreign key here

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; }
    }

}
