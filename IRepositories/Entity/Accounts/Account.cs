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
using IRepositories.Entity.Inventory;

namespace IRepositories.Entity.Accounts
{
    public class Account : BaseEntity
    {
        [ForeignKey("VaccineCenter")]
        public string? VaccineCenterId { get; set; }  // Foreign key for VaccineCenter

        public virtual VaccineCenter VaccineCenter { get; set; }  // Navigation property

        public string? Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleEnum Role { get; set; }

        public string PhoneNumber { get; set; }

        public string ImageProfile {  get; set; }

        public string Status { get; set; }

        public virtual ICollection<ChildrenProfile> ChildrenProfiles { get; set; }
    }

}
