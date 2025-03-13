using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class OrderVaccineDetailResponseDTO
    {
        public string OrderVaccineId { get; set; }
        public string VaccineId { get; set; }
        public string VaccineName { get; set; } // Tên vaccine
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public string Image { get; set; } // Hình ảnh vaccine
    }
}
