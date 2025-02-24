using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace IRepositories.Entity
{
    public class VaccinationSchedule : BaseEntity
    {
        public string ProfileId { get; set; } // This is the foreign key to ChildrenProfile
        public string VaccineCenterId { get; set; }
        public string? OrderVaccineDetailsId { get; set; }
        public string? OrderPackageDetailsId { get; set; }
        public int DoseNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? AdministeredBy { get; set; }
        public int Status { get; set; }

        // Navigation properties
        [ForeignKey("ProfileId")]
        public virtual ChildrenProfile Profile { get; set; }

        [ForeignKey("VaccineCenterId")]
        public virtual VaccineCenter VaccineCenter { get; set; }

        [ForeignKey("OrderVaccineDetailsId")]
        public virtual OrderVaccineDetails OrderVaccineDetails { get; set; }

        [ForeignKey("OrderPackageDetailsId")]
        public virtual OrderPackageDetails OrderPackageDetails { get; set; }

        public virtual ICollection<VaccineReaction> VaccineReactions { get; set; }
    }
}
