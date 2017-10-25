﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SendStoryBusinessLogic
    {
        private ISendStoryView _view;
        private IRouter _router;
        private RemoteStoriesDataService _remoteStoriesDataService;
        private LocalNotificationsDataService _localNotificationsDataService;
        private SendStoryValidator _validator;

        private ICollection<ContactDTO> _recipients;
        public SendStoryBusinessLogic(ISendStoryView _view, IRouter _router, RemoteStoriesDataService remoteStoriesDataService)
        {
            this._view = _view;
            this._router = _router;
            this._remoteStoriesDataService = remoteStoriesDataService;
            _localNotificationsDataService = new LocalNotificationsDataService();
            this._validator = new SendStoryValidator();
        }

        public void Init()
        {
            if (_view.StoryRequest != null)
            {
                this._view.StoryTitle.Text = _view.StoryRequest.Title;
                this._view.StoryTitle.Enabled = false;
                InitButtons();
            }
        }

        public void InitButtons()
        {
            this._view.SendButton.Enabled = (_view.StoryRequest != null || _recipients != null) && !string.IsNullOrWhiteSpace(_view.StoryTitle.Text);
            this._view.ChooseRecipientsButton.Enabled = _view.StoryRequest == null;
        }

        public async Task SendAsync()
        {
            this._view.SendButton.Enabled = false;
            var title = this._view.StoryTitle.Text;

            var videoStream = File.OpenRead(this._view.VideoPath);
            var previewImageStream = File.OpenRead(this._view.PreviewImagePath);
            var uploadResult = await _remoteStoriesDataService
                .UploadMediaAsync(
                    videoStream,
                    Path.GetFileName(this._view.VideoPath),
                    previewImageStream,
                    Path.GetFileName(this._view.PreviewImagePath))
                .ConfigureAwait(false);
            if (uploadResult.IsSuccess)
            {
                var dto = new SendStoryDTO();
                dto.Title = title;
                dto.RequestId = _view.StoryRequest?.Id;
                if (dto.RequestId == null)
                {
                    dto.Receivers = _recipients.Select(x => new StoryReceiverDTO
                    {
                        UserId = x.Type == ContactType.User ? x.User.Id : null,
                        TribeId = x.Type == ContactType.Tribe ? x.Tribe.Id : (int?)null
                    }).ToList();
                }
                dto.VideoUrl = uploadResult.Data.VideoUrl;
                dto.PreviewUrl = uploadResult.Data.PreviewImageUrl;
                dto.NotificationId = _view.RequestNotification?.Id;

                var result = await _remoteStoriesDataService.SendStoryAsync(dto).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    if (_view.RequestNotification != null)
                    {
                        _view.RequestNotification.Handled = true;
                        await _localNotificationsDataService.SaveAsync(_view.RequestNotification).ConfigureAwait(false);
                    }

                    this._view.ShowSuccessMessage("Story successfully sent", _view.Close);
                }
                else
                {
                    uploadResult.ShowResultError(this._view);
                }
            }
            else
            {
                uploadResult.ShowResultError(this._view);
            }

            _view.InvokeOnMainThread(() => this._view.SendButton.Enabled = true);
        }

        public void ChooseRecipients()
        {
            _router.NavigateChooseRecipients(_view, HandleStorytellersSelectedEventHandler, true);
        }

        void HandleStorytellersSelectedEventHandler(ICollection<ContactDTO> selectedContacts)
        {
            _recipients = selectedContacts;
            InitButtons();
        }
    }
}
