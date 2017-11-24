using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.Extensions;

namespace TellMe.Core.Types.BusinessLogic
{
    public class StoriesBusinessLogic : StoriesTableBusinessLogic, IStoriesBusinessLogic
    {
        private readonly IRemoteNotificationsDataService _remoteNotificationsService;

        public StoriesBusinessLogic(IRemoteStoriesDataService remoteStoriesDataService, IRouter router,
            ILocalStoriesDataService localStoriesService, IRemoteNotificationsDataService remoteNotificationsService) :
            base(remoteStoriesDataService, router, localStoriesService)
        {
            _remoteNotificationsService = remoteNotificationsService;
        }

        public new IStoriesListView View { get => (IStoriesListView) base.View;
            set => base.View = value;
        }

        public void SendStory()
        {
            Router.NavigateRecordStory(View);
        }

        public void RequestStory()
        {
            Router.NavigateChooseRecipients(View, RequestStoryRecipientSelectedEventHandler, false);
        }

        public void AccountSettings()
        {
            Router.NavigateAccountSettings(View);
        }

        public void ShowStorytellers()
        {
            Router.NavigateStorytellers(View);
        }

        public void NotificationsCenter()
        {
            Router.NavigateNotificationsCenter(View);
        }

        private void RequestStoryRecipientSelectedEventHandler(IDismissable chooseRecipientsView,
            ICollection<ContactDTO> selectedContacts)
        {
            Router.NavigatePrepareStoryRequest(this.View, selectedContacts,
                (x, y) => CreateStoryRequestAsync(chooseRecipientsView, x, y));
        }

        private async void CreateStoryRequestAsync(IDismissable chooseRecipientsView, RequestStoryDTO dto,
            ICollection<ContactDTO> recipients)
        {
            var overlay = this.View.DisableInput();
            var result = await this.RemoteStoriesDataService.RequestStoryAsync(dto, recipients).ConfigureAwait(false);
            this.View.EnableInput(overlay);
            if (result.IsSuccess)
            {
                this.View.ShowSuccessMessage("Story successfully requested", chooseRecipientsView.Dismiss);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public async Task LoadActiveNotificationsCountAsync()
        {
            var result = await _remoteNotificationsService.GetActiveNotificationsCountAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                this.View.DisplayNotificationsCount(result.Data);
            }
            else
            {
                result.ShowResultError(this.View);
            }
        }

        public void NavigateEvents()
        {
            Router.NavigateEvents(this.View);
        }
    }
}