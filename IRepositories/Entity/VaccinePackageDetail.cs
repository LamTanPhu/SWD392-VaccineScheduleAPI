using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccinePackageDetail
    {
        [Key]
        public Guid VaccinePackageDetailId { get; set; }
        public int FKVaccineId { get; set; }
        public Vaccine Vaccine { get; set; }
        public int FKVaccinePackageId { get; set; }
        public VaccinePackage VaccinePackage { get; set; }
        public int PackagePrice { get; set; }
    }
}
