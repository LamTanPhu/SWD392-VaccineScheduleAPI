using System.ComponentModel.DataAnnotations;

namespace PRN222_Group_Project.Models.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public Guid FKOrderId { get; set; }
        public Order Order { get; set; }
        public string PaymentName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public int PaymentStatus { get; set; }
        public int PayAmount { get; set; }
    }

}
