using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Base;
namespace IRepositories.Entity
{


    public class Manufacturer : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string ActiveStatus { get; set; }
        public string Status { get; set; }

        // Navigation property
        public virtual ICollection<VaccineBatch> VaccineBatches { get; set; }
    }

}
