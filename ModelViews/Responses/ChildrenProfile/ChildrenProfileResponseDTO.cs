using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.ChildrenProfile
{
    public class ChildrenProfileResponseDTO
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string Address { set; get; }
    }
}
