﻿using System;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core
{
    public sealed class App
    {
        private static readonly Lazy<App> Lazy = new Lazy<App>(() => new App());
        public static App Instance => Lazy.Value;

        private App()
        {
        }

        public void Initialize()
        {
        }

        public event Action<Exception> OnException;
        public event Action<Exception> OnNetworkException;
        public event Action<NotificationDTO> OnNotificationReceived;
        public event Action<StoryDTO> OnStoryLikeChanged;


        public void NotificationReceived(NotificationDTO notification)
        {
            this.OnNotificationReceived?.Invoke(notification);
        }

        public void StoryLikeChanged(StoryDTO story)
        {
            this.OnStoryLikeChanged?.Invoke(story);
        }

        public void LogException(Exception ex)
        {
            OnException?.Invoke(ex);
        }

        public void LogNetworkException(Exception ex)
        {
            OnNetworkException?.Invoke(ex);
        }
    }
}