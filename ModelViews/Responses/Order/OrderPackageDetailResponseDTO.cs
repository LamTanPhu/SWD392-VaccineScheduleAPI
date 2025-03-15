using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class OrderPackageDetailResponseDTO
    {
        public string OrderPackageId { get; set; }
        public string VaccinePackageId { get; set; }
        public string VaccinePackageName { get; set; } 
        public string Description { get; set; } 
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
