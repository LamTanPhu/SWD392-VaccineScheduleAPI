using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class Vaccine
    {
        [Key]
        public int VaccineId { get; set; }
        public int FKCategoryId { get; set; }
        public VaccineCategory Category { get; set; }
        public int FKBatchId { get; set; }
        public VaccineBatch Batch { get; set; }
        public string Name { get; set; }
        public int QuantityAvailable { get; set; }
        public int UnitOfVolume { get; set; }
        public string IngredientsDescription { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime BetweenPeriod { get; set; }
        public int Price { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

}
