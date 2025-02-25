using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Manufacturer
{
    public class ManufacturerRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
