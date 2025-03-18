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
        Task<IEnumerable<ChildrenProfileResponseDTO>> GetAllProfilesByAccountIdAsync(string accountId);
        Task<ChildrenProfileResponseDTO?> GetProfileByIdAsync(string id);
        //Task AddProfileAsync(ChildrenProfileRequestDTO profileDto);
        //Task<ChildrenProfileResponseDTO> AddProfileAsync(ChildrenProfileRequestDTO profileDto);
        //Task UpdateProfileAsync(string id, ChildrenProfileRequestDTO profileDto);
        Task UpdateProfileAsync(string id, ChildrenProfileCreateUpdateDTO profileDto);
        Task<ChildrenProfileResponseDTO> AddProfileAsync(string accountId, ChildrenProfileCreateUpdateDTO profileDto);

        Task DeleteProfileAsync(string id);
    }
}
