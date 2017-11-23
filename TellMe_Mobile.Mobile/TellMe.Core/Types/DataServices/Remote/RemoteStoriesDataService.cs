using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteStoriesDataService : IRemoteStoriesDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteStoriesDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<List<StoryRequestDTO>>> RequestStoryAsync(RequestStoryDTO requestStoryDTO,
            ICollection<ContactDTO> contacts)
        {
            var requests = contacts.Select(x => new StoryRequestDTO
            {
                Title = requestStoryDTO.Title,
                UserId = x.Type == ContactType.User ? x.User.Id : null,
                TribeId = x.Type == ContactType.Tribe ? x.Tribe.Id : (int?) null
            }).ToList();
            var result = await this._apiProvider
                .PostAsync<List<StoryRequestDTO>>("stories/request", new {Requests = requests})
                .ConfigureAwait(false);

            return result;
        }

        public async Task<Result<List<StoryDTO>>> GetStoriesAsync(DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider.GetAsync<List<StoryDTO>>($"stories/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<StoryDTO>>> GetStoriesAsync(string userId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider
                .GetAsync<List<StoryDTO>>($"stories/{userId}/older-than/{olderThan.Ticks}").ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<StoryDTO>>> GetStoriesAsync(int tribeId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider
                .GetAsync<List<StoryDTO>>($"stories/tribe/{tribeId}/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<StoryReceiverDTO>>> GetStoryReceiversAsync(int storyId)
        {
            var result = await this._apiProvider.GetAsync<List<StoryReceiverDTO>>($"stories/{storyId}/receivers")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<UploadMediaDTO>> UploadMediaAsync(FileStream videoStream, string videoFileName,
            FileStream previewImageStream, string previewImageFileName)
        {
            videoStream.Position = 0;
            previewImageStream.Position = 0;
            var data = new MultipartFormDataContent();
            data.Add(new StreamContent(videoStream), "VideoFile", videoFileName);
            data.Add(new StreamContent(previewImageStream), "PreviewImageFile", previewImageFileName);

            var result = await this._apiProvider
                .SendDataAsync<UploadMediaDTO>("stories/upload-media", HttpMethod.Post, data).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<StoryDTO>> SendStoryAsync(SendStoryDTO dto)
        {
            var result = await this._apiProvider.PostAsync<StoryDTO>("stories", dto).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<StoryStatus>> RejectStoryRequestAsync(int storyId, int notificationid)
        {
            var result = await this._apiProvider
                .PostAsync<StoryStatus>($"stories/{storyId}/reject-request", notificationid).ConfigureAwait(false);
            return result;
        }

        public async Task<Result> LikeAsync(int storyId)
        {
            var result = await this._apiProvider.PostAsync<object>($"stories/{storyId}/like", null)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result> DislikeAsync(int storyId)
        {
            var result = await this._apiProvider.PostAsync<object>($"stories/{storyId}/dislike", null)
                .ConfigureAwait(false);
            return result;
        }
    }
}