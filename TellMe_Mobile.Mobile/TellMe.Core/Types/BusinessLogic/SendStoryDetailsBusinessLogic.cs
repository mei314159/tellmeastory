using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Validation;

namespace TellMe.Core.Types.BusinessLogic
{
    public class SendStoryDetailsBusinessLogic
    {
        private ISendStoryDetailsView _view;
        private IRouter _router;
        private RemoteStoriesDataService _remoteStoriesDataService;
        private List<ContactDTO> recipientsList;
        private SendStoryValidator _validator;
        public SendStoryDetailsBusinessLogic(ISendStoryDetailsView _view, IRouter _router, RemoteStoriesDataService remoteStoriesDataService)
        {
            this._view = _view;
            this._router = _router;
            this._remoteStoriesDataService = remoteStoriesDataService;
            this._validator = new SendStoryValidator();
            this.recipientsList = new List<ContactDTO>();
        }

        public void ChooseRecipients()
        {
            _router.NavigateChooseRecipients(_view, RecipientsSelected);
        }

        void RecipientsSelected(ICollection<ContactDTO> selectedItems)
        {
            recipientsList.Clear();
            recipientsList.AddRange(selectedItems);
            this._view.SendButton.Enabled = selectedItems.Count > 0;
            this._view.DisplayRecipients(selectedItems);
        }

        public async Task SendAsync()
        {
            this._view.SendButton.Enabled = false;
            var title = this._view.StoryName.Text;
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
                dto.ReceiverIds = recipientsList.Select(x => x.UserId).ToArray();
                dto.VideoUrl = uploadResult.Data.VideoUrl;
                dto.PreviewUrl = uploadResult.Data.PreviewImageUrl;
                var result = await _remoteStoriesDataService.SendStoryAsync(dto).ConfigureAwait(false);
                if (result.IsSuccess)
                {
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
    }
}
