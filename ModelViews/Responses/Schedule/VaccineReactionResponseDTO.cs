using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Schedule
{
    public class VaccineReactionResponseDTO
    {
        public string Id { get; set; }
        public string VaccinationScheduleId { get; set; }
        public string Reaction { get; set; }
        public string Severity { get; set; }
        public int ReactionTime { get; set; }
        public int? ResolvedTime { get; set; }
        public DateTimeOffset CreatedTime { get; set; }  
        public DateTimeOffset LastUpdatedTime { get; set; }  
    }


}
