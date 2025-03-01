using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Auth
{
    public class GoogleUserInfo
    {
        public string Id { get; set; } // Google User ID
        public string Email { get; set; } // User's Email
        public string Name { get; set; } // User's Full Name
        public string GivenName { get; set; } // First Name
        public string FamilyName { get; set; } // Last Name
        public string Picture { get; set; } // Profile Picture URL
        public string Locale { get; set; } // Locale (Language / Region)
    }
}
