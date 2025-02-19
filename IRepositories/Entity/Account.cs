﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Base;
namespace IRepositories.Entity
{


    public class Account : BaseEntity
    {
        public string CenterId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // e.g., Admin, Parent, Doctor

        // Navigation properties
        public virtual VaccineCenter Center { get; set; }
        public virtual ICollection<ChildrenProfile> ChildrenProfiles { get; set; }
    }

}
