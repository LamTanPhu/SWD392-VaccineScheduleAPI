using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccinePackage
{
    public class VaccinePackageResponseDTO
    {
        public string Id { get; set; }
        public string PackageName { get; set; }
        public string? PackageDescription { get; set; }
        public string PackageStatus { get; set; } // Đồng bộ với entity dùng string
        public int PackagePrice { get; set; }
        public List<VaccineWithDoseResponseDTO> Vaccines { get; set; } = new List<VaccineWithDoseResponseDTO>();
    }
}


