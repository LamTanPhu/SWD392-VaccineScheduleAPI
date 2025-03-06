using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class OrderPackageDetailsResponseDTO
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string VaccinePackageId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
