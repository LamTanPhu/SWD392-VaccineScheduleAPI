using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccinePackage
{
    public class VaccinePackageDetailsRequestDTO
    {
        [Required]
        public string VaccineId { get; set; }
        [Required]
        public string VaccinePackageId { get; set; }
        [Required]
        public int PackagePrice { get; set; }
    }
}
