using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccinePackage
    {
        [Key]
        public int VaccinePackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageDescription { get; set; }
        public int PackageStatus { get; set; }
    }
}
