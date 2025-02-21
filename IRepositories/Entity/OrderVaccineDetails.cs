using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{
    public class OrderVaccineDetails : BaseEntity
    {
        [ForeignKey("Order")]
        public string OrderId { get; set; }
        [ForeignKey("Vaccine")]
        public string VaccineId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Vaccine Vaccine { get; set; }
    }
}
