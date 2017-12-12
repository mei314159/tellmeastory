using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using TellMe.Shared.Contracts.Enums;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class SendStoryBusinessLogic : ISendStoryBusinessLogic
    {
        private readonly IRouter _router;
        private readonly IRemoteStoriesDataService _remoteStoriesDataService;
        private readonly ILocalNotificationsDataService _localNotificationsDataService;
        private readonly ILocalAccountService _localAccountService;
        private ICollection<ContactDTO> _recipients;

        public SendStoryBusinessLogic(IRouter router, IRemoteStoriesDataService remoteStoriesDataService,
            ILocalNotificationsDataService localNotificationsDataService, ILocalAccountService localAccountService)
        {
            this._router = router;
            this._remoteStoriesDataService = remoteStoriesDataService;
            _localNotificationsDataService = localNotificationsDataService;
            _localAccountService = localAccountService;
        }

        public ISendStoryView View { get; set; }

        public void Init()
        {
            if (View.StoryRequest != null)
            {
                this.View.StoryTitle.Text = View.StoryRequest.Title;
                this.View.StoryTitle.Enabled = false;
            }
            else if (View.Contact != null)
            {
                _recipients = new[] {View.Contact};
            }

            InitButtons();
        }

        public void InitButtons()
        {
            this.View.SendButton.Enabled = (View.StoryRequest != null || _recipients != null) &&
                                           !string.IsNullOrWhiteSpace(View.StoryTitle.Text);
            this.View.ChooseRecipientsButton.Enabled = View.StoryRequest == null;
            if (View.Contact != null)
            {
                View.ChooseRecipientsButton.Hidden = true;
            }
        }

        public async Task SendAsync()
        {
            this.View.SendButton.Enabled = false;
            var title = this.View.StoryTitle.Text;

            var videoStream = File.OpenRead(this.View.VideoPath);
            var previewImageStream = File.OpenRead(this.View.PreviewImagePath);
            var uploadResult = await _remoteStoriesDataService
                .UploadMediaAsync(
                    videoStream,
                    Path.GetFileName(this.View.VideoPath),
                    previewImageStream,
                    Path.GetFileName(this.View.PreviewImagePath))
                .ConfigureAwait(false);
            if (uploadResult.IsSuccess)
            {
                var dto = new SendStoryDTO
                {
                    Title = title,
                    RequestId = View.StoryRequest?.Id
                };
                if (dto.RequestId == null)
                {
                    dto.Receivers = _recipients.Select(x => new StoryReceiverDTO
                    {
                        UserId = x.Type == ContactType.User ? x.User.Id : null,
                        TribeId = x.Type == ContactType.Tribe ? x.Tribe.Id : (int?) null
                    }).ToList();
                }
                dto.VideoUrl = uploadResult.Data.VideoUrl;
                dto.PreviewUrl = uploadResult.Data.PreviewImageUrl;
                dto.NotificationId = View.RequestNotification?.Id;
                dto.EventId = View.Event?.Id;
                var result = await _remoteStoriesDataService.SendStoryAsync(dto).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    if (View.RequestNotification != null)
                    {
                        View.RequestNotification.Handled = true;
                        await _localNotificationsDataService.SaveAsync(View.RequestNotification).ConfigureAwait(false);
                    }

                    this.View.ShowSuccessMessage("Story successfully sent", View.Close);
                }
                else
                {
                    uploadResult.ShowResultError(this.View);
                }
            }
            else
            {
                uploadResult.ShowResultError(this.View);
            }

            View.InvokeOnMainThread(() => this.View.SendButton.Enabled = true);
        }

        public void ChooseRecipients()
        {
            _router.NavigateChooseRecipients(View, HandleStorytellersSelectedEventHandler, true);
        }

        public string GetUsername()
        {
            return _localAccountService.GetAuthInfo().Account.UserName;
        }

        private void HandleStorytellersSelectedEventHandler(IDismissable view, ICollection<ContactDTO> selectedContacts)
        {
            _recipients = selectedContacts;
            InitButtons();
        }
    }
}