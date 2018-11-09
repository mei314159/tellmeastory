using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Types.DataServices.Remote
{
    public class RemoteStoriesDataService : IRemoteStoriesDataService
    {
        private readonly IApiProvider _apiProvider;

        public RemoteStoriesDataService(IApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public async Task<Result<List<StoryRequestDTO>>> RequestStoryAsync(RequestStoryDTO dto)
        {
            var result = await this._apiProvider
                .PostAsync<List<StoryRequestDTO>>("stories/request", dto)
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

        public async Task<Result<List<StoryDTO>>> GetEventStoriesAsync(int eventId, DateTime? olderThanUtc = null)
        {
            var olderThan = olderThanUtc ?? DateTime.MaxValue;
            var result = await this._apiProvider
                .GetAsync<List<StoryDTO>>($"stories/event/{eventId}/older-than/{olderThan.Ticks}")
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result<List<StoryListDTO>>> SearchAsync(string fragment, int skip)
        {
            var url = string.IsNullOrWhiteSpace(fragment)
                ? $"stories/search/skip/{skip}"
                : $"stories/search/skip/{skip}/{fragment}";
            var result = await this._apiProvider.GetAsync<List<StoryListDTO>>(url).ConfigureAwait(false);
            return result;
        }


        public async Task<Result<StoryDTO>> GetStoryAsync(int storyId)
        {
            var result = await this._apiProvider.GetAsync<StoryDTO>($"stories/{storyId}").ConfigureAwait(false);
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

        public async Task<Result> FlagAsObjectionableAsync(int storyId)
        {
            var result = await this._apiProvider.PostAsync<object>($"stories/{storyId}/flag-as-objectionable", null)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result> UnflagAsObjectionableAsync(int storyId)
        {
            var result = await this._apiProvider.PostAsync<object>($"stories/{storyId}/unflag-as-objectionable", null)
                .ConfigureAwait(false);
            return result;
        }

        public async Task<Result> AddToPlaylistAsync(int storyId, int playlistId)
        {
            var result = await this._apiProvider.PostAsync<object>($"stories/{storyId}/add-to-playlist/{playlistId}", null)
                .ConfigureAwait(false);
            return result;
        }
    }
}