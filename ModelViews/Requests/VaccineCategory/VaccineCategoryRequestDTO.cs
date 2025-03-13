using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineCategory
{
    public class VaccineCategoryRequestDTO
    {
        [Required]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Status { get; set; }
        public string? ParentCategoryId { get; set; }
    }
}
