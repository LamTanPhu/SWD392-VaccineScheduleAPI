using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccineBatch
    {
        [Key]
        public int VaccineBatchId { get; set; }
        public int FKManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public int FKCenterId { get; set; }
        public VaccineCenter Center { get; set; }
        public int Quantity { get; set; }
        public string ActiveStatus { get; set; }
    }

}
