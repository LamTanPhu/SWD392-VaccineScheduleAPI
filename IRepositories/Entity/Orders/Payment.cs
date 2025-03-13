using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;

namespace IRepositories.Entity.Orders
{

    public class Payment : BaseEntity
    {
        [ForeignKey("Order")]
        public string OrderId { get; set; }

        public string TransactionId { get; set; }
        public string PaymentName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PayAmount { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
    }

}
