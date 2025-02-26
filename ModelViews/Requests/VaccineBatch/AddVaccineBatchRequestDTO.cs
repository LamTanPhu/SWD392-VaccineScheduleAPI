using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineBatch
{
    public class AddVaccineBatchRequestDTO
    {
        public string BatchNumber { get; set; }
        public int Quantity { get; set; }
        public string ManufacturerId { get; set; }
        public string VaccineCenterId { get; set; }
        public string ActiveStatus { get; set; }
    }
}
