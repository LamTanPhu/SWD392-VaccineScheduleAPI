using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{
    // In VaccineBatch class
    public class VaccineBatch : BaseEntity
    {
        [ForeignKey("Manufacturer")]
        public string ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }


        [ForeignKey("VaccineCenter")]
        public string VaccineCenterId { get; set; }
        public virtual VaccineCenter VaccineCenter { get; set; }

        public int Quantity { get; set; }
        public string BatchNumber { get; set; }

        public string ActiveStatus { get; set; }
    }
}
