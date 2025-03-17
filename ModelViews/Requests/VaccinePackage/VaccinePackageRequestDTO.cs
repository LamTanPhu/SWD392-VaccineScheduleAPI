using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ModelViews.Requests.VaccinePackage
{
    public class VaccinePackageRequestDTO
    {
        [Required]
        public string PackageName { get; set; }

        public string? PackageDescription { get; set; }

        [Required]
        public List<string> VaccineIds { get; set; } // THE LIST OF VACCINE IDS TO CREATE THE PACKAGE
    }


}
