using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Local
{
    public interface ILocalStorytellersDataService : ILocalDataService
    {
        Task DeleteAllAsync();
        Task SaveAllAsync(IEnumerable<StorytellerDTO> items);
        Task<DataResult<ICollection<StorytellerDTO>>> GetAllAsync();
        Task SaveAsync(StorytellerDTO storyteller);
        Task<DataResult<StorytellerDTO>> GetAsync(string storytellerId);
    }
}