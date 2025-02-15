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
        public string OrderId { get; set; }
        public string VaccinePackageId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("VaccinePackageId")]
        public virtual VaccinePackage VaccinePackage { get; set; }
    }

}
