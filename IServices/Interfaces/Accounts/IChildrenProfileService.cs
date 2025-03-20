using ModelViews.Requests.ChildrenProfile;
using ModelViews.Responses.ChildrenProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices.Interfaces.Accounts
{
    public interface IChildrenProfileService
    {
        Task<IEnumerable<ChildrenProfileResponseDTO>> GetMyChildrenProfilesAsync(string userEmail);
        Task<ChildrenProfileResponseDTO> CreateProfileAsync(string userEmail, ChildrenProfileCreateUpdateDTO profileDto);
        Task UpdateProfileAsync(string id, string userEmail, ChildrenProfileCreateUpdateDTO profileDto);
        Task DeleteProfileAsync(string id, string userEmail);
    }
}
