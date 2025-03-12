using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Config
{
    public class VNPayConfig
    {
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string BaseUrl { get; set; } // Đổi từ PaymentUrl thành BaseUrl
        public string ReturnUrl { get; set; }
        public string Version { get; set; } // Thêm để dùng vnp_Version
        public string Command { get; set; } // Thêm để dùng vnp_Command
        public string CurrCode { get; set; } // Thêm để dùng vnp_CurrCode
        public string Locale { get; set; } // Thêm để dùng vnp_Locale
    }
}
