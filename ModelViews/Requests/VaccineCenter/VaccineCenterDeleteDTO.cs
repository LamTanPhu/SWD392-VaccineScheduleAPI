using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineCenter
{
    public class VaccineCenterDeleteDTO
    {
        [Required]
        public string Id { get; set; }
    }
}
