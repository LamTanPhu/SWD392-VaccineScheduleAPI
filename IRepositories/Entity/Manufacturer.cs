using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class Manufacturer
    {
        [Key]
        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string ActiveStatus { get; set; }
    }
}
