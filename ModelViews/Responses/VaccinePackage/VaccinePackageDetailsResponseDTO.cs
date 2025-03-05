using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccinePackage
{
    public class VaccinePackageDetailsResponseDTO
    {
        public string Id { get; set; }
        public string VaccineId { get; set; }
        public string VaccinePackageId { get; set; }
        public int PackagePrice { get; set; }
    }
}
