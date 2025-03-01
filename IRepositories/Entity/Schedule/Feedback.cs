using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using IRepositories.Entity.Orders;
namespace IRepositories.Entity.Schedule
{
    public class Feedback : BaseEntity
    {
        [ForeignKey("Order")]
        public string OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string Status { get; set; }
        // Navigation property
        public virtual Order Order { get; set; }
    }
}
