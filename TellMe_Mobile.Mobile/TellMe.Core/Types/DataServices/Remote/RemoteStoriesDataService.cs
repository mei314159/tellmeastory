using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteStoriesDataService : BaseDataService
    {
        public async Task<Result<List<StoryRequestDTO>>> RequestStoryAsync(RequestStoryDTO dto)
        {
            var result = await this.PostAsync<List<StoryRequestDTO>>("stories/request", dto).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<List<StoryDTO>>> GetStoriesAsync(int skip)
		{
            var result = await this.GetAsync<List<StoryDTO>>($"stories/skip/{skip}").ConfigureAwait(false);
			return result;
		}

        public async Task<Result<UploadMediaDTO>> UploadMediaAsync(FileStream videoStream, string videoFileName, FileStream previewImageStream, string previewImageFileName)
        {
			videoStream.Position = 0;
            previewImageStream.Position = 0;
            var data = new MultipartFormDataContent();
			data.Add(new StreamContent(videoStream), "VideoFile", videoFileName);
            data.Add(new StreamContent(previewImageStream), "PreviewImageFile", previewImageFileName);

			var result = await SendDataAsync<UploadMediaDTO>("stories/upload-media", HttpMethod.Post, data).ConfigureAwait(false);
            return result;
        }

        public async Task<Result<StoryDTO>> SendStoryAsync(SendStoryDTO dto)
        {
            var result = await this.PostAsync<StoryDTO>("stories", dto).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<StoryStatus>> RejectStoryRequestAsync(int storyId, int notificationid)
        {
            var result = await this.PostAsync<StoryStatus>($"stories/{storyId}/reject-request", notificationid).ConfigureAwait(false);
            return result;
        }
    }
}
