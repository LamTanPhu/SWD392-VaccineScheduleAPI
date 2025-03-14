using IRepositories.Entity.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ModelViews.Responses.VaccineCenter;
using ModelViews.Responses.ChildrenProfile;
namespace ModelViews.Responses.Auth
{
    public class ProfileResponseDTO
        {
            public string AccountId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }
            public string? PhoneNumber { get; set; }
            public string? ImageProfile { get; set; }
            public VaccineCenterResponseDTO? VaccineCenter { get; set; } // Nullable, full object when present
            public List<ChildrenProfileResponseDTO>? ChildrenProfiles { get; set; } // Nullable, list when presentx
        }
}