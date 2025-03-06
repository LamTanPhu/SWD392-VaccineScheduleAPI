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
        Task<IEnumerable<ChildrenProfileResponseDTO>> GetAllProfilesAsync();
        Task<ChildrenProfileResponseDTO?> GetProfileByIdAsync(string id);
        Task AddProfileAsync(ChildrenProfileRequestDTO profile);
        Task UpdateProfileAsync(string id, ChildrenProfileRequestDTO profileDto);
        Task DeleteProfileAsync(string id);
    }
}
