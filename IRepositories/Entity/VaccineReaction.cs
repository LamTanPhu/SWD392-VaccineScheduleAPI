using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccineReaction
    {
        [Key]
        public Guid VaccineReactionId { get; set; }
        public Guid FKVaccineScheduleId { get; set; }
        public VaccinationSchedule VaccineSchedule { get; set; }
        public string Reaction { get; set; }
        public int Severity { get; set; }
        public int ReactionTime { get; set; }
        public int ResolvedTime { get; set; }
    }
}
