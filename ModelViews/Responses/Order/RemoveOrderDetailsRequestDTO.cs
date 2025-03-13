using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class RemoveOrderDetailsRequestDTO
    {
        public string OrderId { get; set; }
        public List<string> VaccineDetailIds { get; set; } = new List<string>();
        public List<string> PackageDetailIds { get; set; } = new List<string>();
    }
}
