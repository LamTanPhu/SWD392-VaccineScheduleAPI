using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{


    public class Vaccine : BaseEntity
    {
        public string CategoryId { get; set; }
        public string BatchId { get; set; }
        public string Name { get; set; }
        public int QuantityAvailable { get; set; }
        public int UnitOfVolume { get; set; }
        public string? IngredientsDescription { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime BetweenPeriod { get; set; }
        public int Price { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual VaccineCategory Category { get; set; }

        [ForeignKey("BatchId")]
        public virtual VaccineBatch Batch { get; set; }
    }

}
