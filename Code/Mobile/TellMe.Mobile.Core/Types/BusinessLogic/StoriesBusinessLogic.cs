using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.Mobile.Core.Types.BusinessLogic
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

        public new IStoriesListView View
        {
            get => (IStoriesListView) base.View;
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
            Router.NavigatePrepareStoryRequest(this.View, selectedContacts, (item, state) => chooseRecipientsView.Dismiss());
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

        public void NavigatePlaylists()
        {
            Router.NavigatePlaylists(this.View);
        }
    }
}