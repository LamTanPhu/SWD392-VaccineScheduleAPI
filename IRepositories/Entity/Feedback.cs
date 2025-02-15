using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Base;
namespace IRepositories.Entity
{


    public class Feedback : BaseEntity
    {
        public string OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
    }

}
