using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Schedule
{
    public class UpdateScheduleRequestDTO
    {
        public int DoseNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? AdministeredBy { get; set; }
        public int Status { get; set; } // 0: hủy lịch, 1: đặt lịch nhưng chưa đến, 2: đến rồi
    }
}
