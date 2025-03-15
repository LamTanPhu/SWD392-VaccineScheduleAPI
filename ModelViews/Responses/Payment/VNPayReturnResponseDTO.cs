using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VNPay
{
    public class VNPayReturnResponseDTO
    {
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}
