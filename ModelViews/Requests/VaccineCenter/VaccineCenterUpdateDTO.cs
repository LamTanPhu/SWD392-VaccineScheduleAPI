using System.ComponentModel.DataAnnotations;

namespace ModelViews.Requests.VaccineCenter
{
    public class VaccineCenterUpdateDTO
    {
        [Required]
        public string Id { get; set; } // Needed to find the record

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
