using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccinePackage
{
    // DTO cho Add/Remove Vaccine trong Update
    public class VaccinePackageUpdateRequestDTO
    {
        [Required]
        public string VaccineId { get; set; }

        [Required]
        public int DoseNumber { get; set; }
    }
}
