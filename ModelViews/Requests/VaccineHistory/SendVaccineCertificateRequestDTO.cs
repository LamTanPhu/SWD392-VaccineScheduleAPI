using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.VaccineHistory
{
    public class SendVaccineCertificateRequestDTO
    {
        [Required]
        public string ProfileId { get; set; }
        [Required]
        public IFormFile DocumentationProvided { get; set; } // Nhận file ảnh thay vì string
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
    }
}
