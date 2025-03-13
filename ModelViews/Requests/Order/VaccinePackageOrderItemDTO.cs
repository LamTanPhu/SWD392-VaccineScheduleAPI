using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
    public class VaccinePackageOrderItemDTO
    {
        [Required(ErrorMessage = "VaccinePackageId là bắt buộc")]
        public string VaccinePackageId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
