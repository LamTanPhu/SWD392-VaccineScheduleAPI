using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;

namespace IRepositories.Entity
{

    public class Payment : BaseEntity
    {
        public string OrderId { get; set; }
        public string PaymentName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public decimal PayAmount { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
    }

}
