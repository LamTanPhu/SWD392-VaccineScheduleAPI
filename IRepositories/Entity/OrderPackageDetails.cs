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

    public class OrderPackageDetails : BaseEntity
    {
        [ForeignKey("Order")]
        public string OrderId { get; set; }  // Foreign key to the Order table

        [ForeignKey("VaccinePackage")]
        public string VaccinePackageId { get; set; }  // Foreign key to the VaccinePackage table

        public int Quantity { get; set; }
        public int TotalPrice { get; set; }

        // Navigation properties: No need for [ForeignKey] here
        public virtual Order Order { get; set; }  // Navigation property to Order

        public virtual VaccinePackage VaccinePackage { get; set; }  // Navigation property to VaccinePackage
    }


}
