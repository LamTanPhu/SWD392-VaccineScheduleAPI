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
        [Required]
        public string ProfileId { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int TotalAmount { get; set; }
        [Required]
        public int TotalOrderPrice { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
