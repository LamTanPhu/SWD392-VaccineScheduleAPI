using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
    public class AddOrderVaccineDetailsRequestDTO
    {
        public string VaccineId { get; set; }
        public int Quantity { get; set; }
    }
}
