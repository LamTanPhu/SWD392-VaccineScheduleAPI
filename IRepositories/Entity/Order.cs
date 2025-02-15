using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{
    public class Order : BaseEntity
    {
        public string? FeedbackId { get; set; }
        public string ProfileId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int TotalAmount { get; set; }
        public int TotalOrderPrice { get; set; }
        public int Status { get; set; }

        // Navigation properties
        [ForeignKey("FeedbackId")]
        public virtual Feedback Feedback { get; set; }

        [ForeignKey("ProfileId")]
        public virtual ChildrenProfile Profile { get; set; }

        public virtual ICollection<OrderVaccineDetails> OrderVaccineDetails { get; set; }
        public virtual ICollection<OrderPackageDetails> OrderPackageDetails { get; set; }
    }

}
