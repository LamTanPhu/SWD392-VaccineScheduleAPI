using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccineHistory
{
    public class CreateVaccineHistoryResponseDTO
    {
        public string Id { get; set; }
        public string ProfileId { get; set; }
        public string DocumentationProvided { get; set; }
        public string Notes { get; set; }
        public int VerifiedStatus { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
