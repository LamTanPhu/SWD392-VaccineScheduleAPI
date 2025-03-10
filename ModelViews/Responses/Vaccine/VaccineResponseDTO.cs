using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Vaccine
{
    public class VaccineResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? IngredientsDescription { get; set; }
        public int UnitOfVolume { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public DateTime BetweenPeriod { get; set; }
        public int QuantityAvailable { get; set; }
        public int Price { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; } // "1" = active, "0" = inactive
        public string VaccineCategoryId { get; set; }
        public string BatchId { get; set; }
        public string Image { get; set; }
    }
}
