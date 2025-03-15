using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineHistory
{
    public class VerifyVaccineHistoryRequestDTO
    {
        public string VaccineHistoryId { get; set; }
        public int VerifiedStatus { get; set; } // 1 = Duyệt, 2 = Từ chối
        public string Notes { get; set; } // Ghi chú từ Staff (nếu có)
    }
}
