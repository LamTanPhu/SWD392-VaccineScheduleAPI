using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
   public class OrderResponseDTO
    {
        public string OrderId { get; set; }
        public string ProfileId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int TotalAmount { get; set; }
        public int TotalOrderPrice { get; set; }
        public string Status { get; set; }
        public List<OrderVaccineDetailResponseDTO> VaccineDetails { get; set; } = new List<OrderVaccineDetailResponseDTO>();
        public List<OrderPackageDetailResponseDTO> PackageDetails { get; set; } = new List<OrderPackageDetailResponseDTO>();
    }
}
