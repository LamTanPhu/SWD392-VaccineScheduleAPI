using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Order
{
     public class OrderRequestDTO
     {
        [Required(ErrorMessage = "ProfileId là bắt buộc")]
        public string ProfileId { get; set; } // ID của trẻ (ChildrenProfiles)

        public List<VaccineOrderItemDTO> Vaccines { get; set; } = new List<VaccineOrderItemDTO>();

        public List<VaccinePackageOrderItemDTO> VaccinePackages { get; set; } = new List<VaccinePackageOrderItemDTO>();

        [Required(ErrorMessage = "PurchaseDate là bắt buộc")]
        public DateTime PurchaseDate { get; set; }
    }

}

