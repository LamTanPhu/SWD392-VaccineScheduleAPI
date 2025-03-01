using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Base;
namespace IRepositories.Entity.Inventory
{


    public class Manufacturer : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public bool ActiveStatus { get; set; }

        // Navigation property
        public virtual ICollection<VaccineBatch> VaccineBatches { get; set; }
    }

}
