using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ModelViews.Requests.ChildrenProfile
{
    public class ChildrenProfileCreateUpdateDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Status { get; set; }
        public string Address { get; set; } // Optional
    }
}