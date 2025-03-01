using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Base;
namespace IRepositories.Entity.Vaccines
{


    public class VaccinePackage : BaseEntity
    {
        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public bool PackageStatus { get; set; }

        // Navigation property
        public virtual ICollection<VaccinePackageDetail> PackageDetails { get; set; }
    }

}
