using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace IRepositories.Entity.Schedule
{


    public class VaccineReaction : BaseEntity
    {
        public string VaccinationScheduleId { get; set; }  // Updated foreign key property

        [ForeignKey("VaccinationScheduleId")]
        public virtual VaccinationSchedule VaccinationSchedule { get; set; }  // Updated navigation property

        public string Reaction { get; set; }
        public string Severity { get; set; }
        public int ReactionTime { get; set; }
        public int? ResolvedTime { get; set; }
    }

}
