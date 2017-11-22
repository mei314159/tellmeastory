using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.DataServices.Remote
{
    public interface IRemoteStoriesDataService : IRemoteDataService
    {
        Task<Result<List<StoryRequestDTO>>> RequestStoryAsync(RequestStoryDTO dto);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(DateTime? olderThanUtc = null);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(string userId, DateTime? olderThanUtc = null);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(int tribeId, DateTime? olderThanUtc = null);

        Task<Result<UploadMediaDTO>> UploadMediaAsync(FileStream videoStream, string videoFileName,
            FileStream previewImageStream, string previewImageFileName);

        Task<Result<StoryDTO>> SendStoryAsync(SendStoryDTO dto);
        Task<Result<StoryStatus>> RejectStoryRequestAsync(int storyId, int notificationid);
        Task<Result> LikeAsync(int storyId);
        Task<Result> DislikeAsync(int storyId);
    }
}