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
        [ForeignKey("Center")]
        public string CenterId { get; set; }
        public int Quantity { get; set; }
        public string ActiveStatus { get; set; }
        // Navigation properties
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual VaccineCenter Center { get; set; }
    }

}
