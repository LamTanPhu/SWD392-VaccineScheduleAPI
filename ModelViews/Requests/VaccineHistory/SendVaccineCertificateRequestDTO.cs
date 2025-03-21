using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineHistory
{
    public class AddVaccineHistoryRequestDTO
    {
        public string ProfileId { get; set; }
        public string DocumentationProvided { get; set; } // Đường dẫn file ảnh
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
    }
}
