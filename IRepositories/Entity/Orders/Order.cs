using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using IRepositories.Entity.Accounts;
using IRepositories.Entity.Schedules;
namespace IRepositories.Entity.Orders
{
    public class Order : BaseEntity
    {
        [ForeignKey("Profile")]
        public string ProfileId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int TotalAmount { get; set; }
        public int TotalOrderPrice { get; set; }
        public string Status { get; set; } // 1: Pending, 2:Completed, 3:PayLater
        // Navigation properties
        public virtual Feedback Feedback { get; set; }
        [ForeignKey("ProfileId")]
        public virtual ChildrenProfile Profile { get; set; }
        public virtual ICollection<OrderVaccineDetails> OrderVaccineDetails { get; set; }
        public virtual ICollection<OrderPackageDetails> OrderPackageDetails { get; set; }
    }

}
