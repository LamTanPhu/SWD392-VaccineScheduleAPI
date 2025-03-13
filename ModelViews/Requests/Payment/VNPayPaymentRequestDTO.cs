using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VNPay
{
    public class VNPayPaymentRequestDTO
    {
        public decimal Amount { get; set; } // Số tiền thanh toán (VND)
        public string OrderId { get; set; } // Mã đơn hàng
        public string OrderInfo { get; set; } // Thông tin đơn hàng
    }
}
