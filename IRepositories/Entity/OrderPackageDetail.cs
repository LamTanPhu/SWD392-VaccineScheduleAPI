using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class OrderPackageDetail
    {
        [Key]
        public Guid OrderPackageDetaiId { get; set; }
        public Guid FKOrderId { get; set; }
        public Order Order { get; set; }
        public int FKVaccinePackageId { get; set; }
        public VaccinePackage VaccinePackage { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
