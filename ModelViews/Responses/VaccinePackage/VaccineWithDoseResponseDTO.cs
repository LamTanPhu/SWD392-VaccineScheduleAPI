using ModelViews.Responses.Vaccine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccinePackage
{
    public class VaccineWithDoseResponseDTO : VaccineResponseDTO
    {
        public int DoseNumber { get; set; }
    }
}
