using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Dashboard
{
    public class RevenueAndOrderResponseDTO
    {
        public string Period { get; set; } // e.g., "2025-03-21" for day, "2025-03" for month, "2025" for year
        public int TotalOrders { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int VaccineQuantity { get; set; }
        public string PaymentName { get; set; } // Assuming this is the most common payment method or a summary
    }
}
