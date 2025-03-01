using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Manufacturer
{
    public class ManufacturerResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
