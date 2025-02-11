using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class Feedback
    {
        [Key]
        public Guid FeedbackId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
