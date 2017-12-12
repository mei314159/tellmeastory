using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Local
{
    public interface ILocalStoriesDataService : ILocalDataService
    {
        Task DeleteAllAsync();
        Task SaveStoriesAsync(ICollection<StoryDTO> stories);
        Task<DataResult<ICollection<StoryDTO>>> GetAllAsync();
        Task SaveAsync(StoryDTO story);
    }
}