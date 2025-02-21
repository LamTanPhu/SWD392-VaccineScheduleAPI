using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Base;

namespace IRepositories.Entity
{

    public class VaccinePackageDetail : BaseEntity
    {
        [ForeignKey("Vaccine")]
        public string VaccineId { get; set; }
        [ForeignKey("VaccinePackage")]
        public string VaccinePackageId { get; set; }
        public int PackagePrice { get; set; }
        // Navigation properties
        public virtual Vaccine Vaccine { get; set; }
        public virtual VaccinePackage VaccinePackage { get; set; }
    }

}
