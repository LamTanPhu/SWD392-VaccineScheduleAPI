using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity
{


    public class VaccineReaction : BaseEntity
    {
        public string VaccineScheduleId { get; set; }
        public string Reaction { get; set; }
        public int Severity { get; set; }
        public int ReactionTime { get; set; }
        public int? ResolvedTime { get; set; }

        // Navigation property
        [ForeignKey("VaccineScheduleId")]
        public virtual VaccinationSchedule VaccineSchedule { get; set; }
    }

}
