using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteStoriesDataService : BaseDataService
    {
        public async Task<Result> RequestStoryAsync(StoryRequestDTO dto)
        {
            var result = await this.PostAsync<List<ContactDTO>>("stories/request", dto).ConfigureAwait(false);

            return result;
        }
    }
}
