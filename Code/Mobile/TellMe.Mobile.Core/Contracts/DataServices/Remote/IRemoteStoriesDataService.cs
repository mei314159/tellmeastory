﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Shared.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices.Remote
{
    public interface IRemoteStoriesDataService : IRemoteDataService
    {
        Task<Result<List<StoryRequestDTO>>> RequestStoryAsync(RequestStoryDTO requestStoryDTO, ICollection<ContactDTO> contacts);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(DateTime? olderThanUtc = null);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(string userId, DateTime? olderThanUtc = null);
        Task<Result<List<StoryDTO>>> GetStoriesAsync(int tribeId, DateTime? olderThanUtc = null);
        Task<Result<List<StoryDTO>>> GetEventStoriesAsync(int eventId, DateTime? olderThanUtc = null);
        Task<Result<List<StoryListDTO>>> SearchAsync(string fragment, int skip);
        Task<Result<StoryDTO>> GetStoryAsync(int storyId);

        Task<Result<UploadMediaDTO>> UploadMediaAsync(FileStream videoStream, string videoFileName,
            FileStream previewImageStream, string previewImageFileName);

        Task<Result<StoryDTO>> SendStoryAsync(SendStoryDTO dto);
        Task<Result<StoryStatus>> RejectStoryRequestAsync(int storyId, int notificationid);
        Task<Result> LikeAsync(int storyId);
        Task<Result> DislikeAsync(int storyId);
    }
}