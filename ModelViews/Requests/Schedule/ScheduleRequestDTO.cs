using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Schedule
{
    public class ScheduleRequestDTO
    {
        public string OrderId { get; set; }
        public List<ScheduleItemRequestDTO> Schedules { get; set; }
    }
}
