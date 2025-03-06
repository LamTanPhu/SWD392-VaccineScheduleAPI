using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
   public class OrderResponseDTO
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int TotalAmount { get; set; }
        public int TotalOrderPrice { get; set; }
        public string Status { get; set; }
    }
}
