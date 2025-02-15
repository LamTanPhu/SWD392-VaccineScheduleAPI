using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;
using Core.Base;

namespace IRepositories.Entity
{


    public class VaccineCategory : BaseEntity
    {
        public string? ParentCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }

        // Navigation properties
        [ForeignKey("ParentCategoryId")]
        public virtual VaccineCategory ParentCategory { get; set; }

        public virtual ICollection<Vaccine> Vaccines { get; set; }
    }

}
