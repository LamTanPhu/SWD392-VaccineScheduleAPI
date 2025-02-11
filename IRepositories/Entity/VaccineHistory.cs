using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccineHistory
    {
        [Key]
        public Guid VacineHistoryId { get; set; }
        public int FKVaccineId { get; set; }
        public Vaccine Vaccine { get; set; }
        public Guid FKProfileId { get; set; }
        public ChildrenProfile Profile { get; set; }
        public Guid FKAccountId { get; set; }
        public Account Account { get; set; }
        public int FKCenterId { get; set; }
        public VaccineCenter Center { get; set; }
        public DateTime AdministeredDate { get; set; }
        public string AdministeredBy { get; set; }
        public string DocumentationProvided { get; set; }
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
    }
}
