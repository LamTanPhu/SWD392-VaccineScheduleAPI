using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Payment
{
    public class PayAtFacilityRequestDTO
    {
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; } // e.g., "Cash", "Card"
    }
}
