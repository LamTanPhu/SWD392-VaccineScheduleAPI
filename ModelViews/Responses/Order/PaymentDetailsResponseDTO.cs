using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Order
{
    public class PaymentDetailsResponseDTO
    {

        public string OrderId { get; set; }

        public string PaymentId { get; set; }
        public string TransactionId { get; set; }

        public string PaymentName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PayAmount { get; set; }
    }
}
