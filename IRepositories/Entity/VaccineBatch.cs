using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{


    public class VaccineBatch : BaseEntity
    {
        public string ManufacturerId { get; set; }
        public string CenterId { get; set; }
        public int Quantity { get; set; }
        public string ActiveStatus { get; set; }

        // Navigation properties
        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; }

        [ForeignKey("CenterId")]
        public virtual VaccineCenter Center { get; set; }
    }

}
