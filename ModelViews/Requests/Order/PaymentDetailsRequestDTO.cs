using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelViews.Requests.Order
{
    public class PaymentDetailsRequestDTO
    {
        [Required]
        public string OrderId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PaymentName { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentStatus { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Range(0, double.MaxValue, ErrorMessage = "PayAmount must be a positive value.")]
        public decimal PayAmount { get; set; }
    }
}
