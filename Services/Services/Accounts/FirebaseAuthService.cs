//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Google.Apis.Auth;
//using Microsoft.Extensions.Configuration;
//using System.Threading.Tasks;
//using IServices.Interfaces.Accounts;
//namespace Services.Services.Accounts
//{
//    public class FirebaseAuthService : IFirebaseAuthService
//    {
//        private readonly IConfiguration _configuration;

//        public FirebaseAuthService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<GoogleJsonWebSignature.Payload> VerifyFirebaseTokenAsync(string idToken)
//        {
//            var settings = new GoogleJsonWebSignature.ValidationSettings
//            {
//                Audience = new[] { _configuration["Firebase:Audience"] }
//            };

//            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
//        }
//    }

//}
