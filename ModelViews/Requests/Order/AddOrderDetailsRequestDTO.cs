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
        public List<AddOrderVaccineDetailsRequestDTO> Vaccines { get; set; } = new List<AddOrderVaccineDetailsRequestDTO>();
        public List<AddOrderPackageDetailsRequestDTO> Packages { get; set; } = new List<AddOrderPackageDetailsRequestDTO>();
    }
}
