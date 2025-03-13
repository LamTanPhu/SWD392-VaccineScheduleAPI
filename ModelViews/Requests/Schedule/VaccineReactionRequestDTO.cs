using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Requests.Schedule
{
    public class VaccineReactionRequestDTO
    {
        public string VaccinationScheduleId { get; set; }
        public string Reaction { get; set; }
        public string Severity { get; set; }
        public int ReactionTime { get; set; }
        public int? ResolvedTime { get; set; }
    }
}
