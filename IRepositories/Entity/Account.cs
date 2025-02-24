using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Core.Base;
using IRepositories.Enum;

namespace IRepositories.Entity
{
    public class Account : BaseEntity
    {
        [ForeignKey("VaccineCenter")]
        public string? CenterId { get; set; }
        public string? Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]  // Ensures JSON serialization uses string values
        public RoleEnum Role { get; set; }  // Now using Enum instead of string

        public string Status { get; set; }

        // Navigation properties
        public virtual VaccineCenter Center { get; set; }
        public virtual ICollection<ChildrenProfile> ChildrenProfiles { get; set; }
    }
}
