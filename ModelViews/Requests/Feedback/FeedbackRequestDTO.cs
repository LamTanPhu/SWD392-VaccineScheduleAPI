using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Feedback
{
    public class FeedbackRequestDTO
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public int Rating { get; set; }
        public string? Comment { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
