using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Feedback
{
    public class FeedbackResponseDTO
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string Status { get; set; }
    }
}
