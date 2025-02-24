using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccinePackage
{
    public class VaccinePackageResponseDTO
    {
        public String Id { get; set; }
        public string PackageName { get; set; }
        public string? PackageDescription { get; set; }
        public bool PackageStatus { get; set; }
    }
}
