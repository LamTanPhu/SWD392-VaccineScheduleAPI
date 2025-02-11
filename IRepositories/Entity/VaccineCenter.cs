using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccineCenter
    {
        [Key]
        public int VacineCenterId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int ContactNumber { get; set; }
        public string Email { get; set; }
    }

}
