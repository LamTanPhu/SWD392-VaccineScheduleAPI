using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class VaccineCategory
    {
        [Key]
        public int VaccineCategoryId { get; set; }
        public int? FKParentCategoryId { get; set; }
        public VaccineCategory ParentCategory { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
