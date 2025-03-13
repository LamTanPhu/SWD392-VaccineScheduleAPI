using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
    public class AddOrderDetailsRequestDTO
    {
        public string OrderId { get; set; }
        public List<OrderVaccineDetailsRequestDTO> Vaccines { get; set; } = new List<OrderVaccineDetailsRequestDTO>();
        public List<OrderPackageDetailsRequestDTO> Packages { get; set; } = new List<OrderPackageDetailsRequestDTO>();
    }
}
