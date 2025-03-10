using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class OrderVaccineDetailResponseDTO
    {
        public string VaccineId { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}
