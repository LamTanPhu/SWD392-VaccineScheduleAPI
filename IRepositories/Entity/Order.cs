using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public Guid FKFeedbackId { get; set; }
        public Feedback Feedback { get; set; }
        public Guid FKProfileId { get; set; }
        public ChildrenProfile Profile { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Status { get; set; }
    }
}
