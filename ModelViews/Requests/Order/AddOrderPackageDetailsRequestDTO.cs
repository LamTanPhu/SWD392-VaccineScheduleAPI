using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
    public class AddOrderPackageDetailsRequestDTO
    {

        [Required]
        public string VaccinePackageId { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}
