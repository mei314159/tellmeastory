﻿using System.Threading.Tasks;
using System.Collections.Generic;
using TellMe.DAL.Types.Domain;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.DAL.Contracts.PushNotification
{
    public interface IPushNotificationsService
    {
        Task RegisterPushTokenAsync(string token, string oldToken, OsType osType, string userId, string appVersion);
        
		Task SendStoryRequestPushNotificationAsync(IReadOnlyCollection<StoryDTO> storyDTOs, string requestSenderId);
        Task SendStoryPushNotificationAsync(IReadOnlyCollection<StoryDTO> storyDTOs, string senderId);
    }
}