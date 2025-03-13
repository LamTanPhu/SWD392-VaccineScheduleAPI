using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IFirebaseAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyFirebaseTokenAsync(string idToken);
    }

}
