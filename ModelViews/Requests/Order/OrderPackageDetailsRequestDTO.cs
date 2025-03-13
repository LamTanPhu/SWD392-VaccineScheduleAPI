using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
    public class OrderPackageDetailsRequestDTO
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string VaccinePackageId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int TotalPrice { get; set; }
    }
}
