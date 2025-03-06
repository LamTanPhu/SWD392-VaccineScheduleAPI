using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Vaccine
{
    public class VaccineRequestDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int QuantityAvailable { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string VaccineCategoryId { get; set; }
        [Required]
        public string BatchId { get; set; }
    }
}
