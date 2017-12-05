using System.Collections.Generic;
using System.Linq;
using TellMe.iOS.Controllers;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Core
{
    public class Router : IRouter
    {
        private readonly UIWindow _window;

        public Router(UIWindow window)
        {
            this._window = window;
        }

        public void NavigateMain()
        {
            this._window.InvokeOnMainThread(() =>
                this._window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController()));
        }

        public void NavigatePrepareStoryRequest(IView view, ICollection<ContactDTO> recipients, StoryRequestCreatedEventHandler e)
        {
            var targetController = (RequestStoryController)UIStoryboard.FromName("Story", null)
                .InstantiateViewController("RequestStoryController");
            targetController.Recipients = recipients;
            targetController.RequestCreated += e;
            this.Present(targetController, view);
        }

        public void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellersViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellersController");
                targetController.Mode = ContactsMode.ChooseRecipient;
                targetController.DismissOnFinish = dismissOnFinish;
                targetController.RecipientsSelected += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateChooseTribeMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish,
            HashSet<string> disabledUserIds = null)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellersViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellersController");
                targetController.Mode = ContactsMode.ChooseTribeMembers;
                targetController.DisabledUserIds = disabledUserIds;
                targetController.DismissOnFinish = dismissOnFinish;
                targetController.RecipientsSelected += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateChooseEventMembers(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish,
            HashSet<string> disabledUserIds = null,
            HashSet<int> disabledTribeIds = null)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellersViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellersController");
                targetController.Mode = ContactsMode.ChooseRecipient;
                targetController.DisabledUserIds = disabledUserIds;
                targetController.DisabledTribeIds = disabledTribeIds;
                targetController.DismissOnFinish = dismissOnFinish;
                targetController.RecipientsSelected += e;
                this.Present(targetController, view);
            });
        }
        public void NavigateRecordStory(IView view, StoryRequestDTO storyRequest = null,
            NotificationDTO notification = null, ContactDTO contact = null)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (UINavigationController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("RecordVideoController");
                var recordController = (RecordVideoController)targetController.ViewControllers.First();
                recordController.StoryRequest = storyRequest;
                recordController.RequestNotification = notification;
                recordController.Contact = contact;
                this.Present(targetController, view, false);
            });
        }

        public void NavigatePreviewStory(IView view, string videoPath, StoryRequestDTO storyRequest = null,
            NotificationDTO notification = null, ContactDTO contact = null)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (PreviewVideoController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("PreviewVideoController");
                targetController.VideoPath = videoPath;
                targetController.StoryRequest = storyRequest;
                targetController.RequestNotification = notification;
                targetController.Contact = contact;
                this.Present(targetController, view);
            });
        }

        public void NavigateStoryDetails(IView view, string videoPath, string previewImagePath,
            StoryRequestDTO storyRequest = null, NotificationDTO notification = null, ContactDTO contact = null)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (SendStoryViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("SendStoryViewController");
                targetController.VideoPath = videoPath;
                targetController.PreviewImagePath = previewImagePath;
                targetController.StoryRequest = storyRequest;
                targetController.RequestNotification = notification;
                targetController.Contact = contact;
                this.Present(targetController, view);
            });
        }

        public void NavigateAccountSettings(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Main", null)
                    .InstantiateViewController("ProfileViewController");
                this.Present(targetController, view);
            });
        }

        public void NavigateSetProfilePicture(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Auth", null)
                    .InstantiateViewController("UploadPictureController");
                this.Present(targetController, view);
            });
        }

        public void NavigateStorytellers(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellersController");
                this.Present(targetController, view);
            });
        }

        public void NavigateNotificationsCenter(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {

                var targetController = UIStoryboard.FromName("Main", null)
                    .InstantiateViewController("NotificationCenterController");
                var navigationController = new UINavigationController(targetController);
                this.Present(navigationController, view, false);
            });
        }

        public void NavigateCreateTribe(IView view, ICollection<StorytellerDTO> tribeMembers,
            TribeCreatedHandler complete)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = new CreateTribeController(tribeMembers);
                targetController.TribeCreated += complete;
                this.Present(targetController, view);
            });
        }

        public void NavigateViewStory(IView view, StoryDTO story, bool goToComments = false)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StoryViewController)UIStoryboard.FromName("Main", null)
                    .InstantiateViewController("StoryViewController");
                targetController.Story = story;
                targetController.DisplayCommentsWhenAppear = goToComments;
                targetController.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
                targetController.Parent = view;
                this.Present(targetController, view, false);
            });
        }

        public void NavigateStoryteller(IView view, StorytellerDTO storyteller)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellerViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellerViewController");
                targetController.Storyteller = storyteller;
                this.Present(targetController, view);
            });
        }

        public void NavigateStoryteller(IView view, string userId)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellerViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("StorytellerViewController");
                targetController.StorytellerId = userId;
                this.Present(targetController, view);
            });
        }

        public void NavigateTribeInfo(IView view, TribeDTO tribe, TribeLeftHandler e)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = new TribeInfoViewController(tribe);
                targetController.TribeLeft += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateTribe(IView view, TribeDTO tribe, TribeLeftHandler e)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (TribeViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("TribeViewController");
                targetController.Tribe = tribe;
                targetController.TribeLeft += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateTribe(IView view, int tribeId, TribeLeftHandler e)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (TribeViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("TribeViewController");
                targetController.TribeId = tribeId;
                targetController.TribeLeft += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateViewEvent(IView view, EventDTO eventDTO, EventDeletedHandler eventDeleted)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = (EventViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("EventViewController");
                targetController.Event = eventDTO;
                targetController.EventDeleted += eventDeleted;
                this.Present(targetController, view);
            });
        }

        public void NavigateCreateEvent(IView view, EventSavedHandler eventSaved, EventDeletedHandler eventDeleted)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = IoC.GetInstance<EditEventController>();
                targetController.EventSaved += eventSaved;
                targetController.EventDeleted += eventDeleted;
                this.Present(targetController, view);
            });
        }

        public void NavigateEditEvent(IView view, EventDTO eventDTO, EventSavedHandler eventSaved, EventDeletedHandler eventDeleted)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = IoC.GetInstance<EditEventController>();
                targetController.Event = eventDTO;
                targetController.EventSaved += eventSaved;
                targetController.EventDeleted += eventDeleted;
                this.Present(targetController, view);
            });
        }

        public void NavigateEvents(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = IoC.GetInstance<EventsViewController>();
                this.Present(targetController, view);
            });
        }

        public void NavigatePlaylists(IView view)
        {
            this._window.InvokeOnMainThread(() =>
            {
                var targetController = IoC.GetInstance<PlaylistsViewController>();
                this.Present(targetController, view);
            });
        }

        public void NavigateCreatePlaylist(IView view, ItemUpdateHandler<PlaylistDTO> eventHandler)
        {
            /*this._window.InvokeOnMainThread(() =>
            {
                var targetController = IoC.GetInstance<EditPlaylistController>();
                targetController.ItemUpdated += eventHandler;
                this.Present(targetController, view);
            });*/
        }

        public void NavigateViewPlaylist(IView view, PlaylistDTO dto, ItemUpdateHandler<PlaylistDTO> eventHandler)
        {
            /*this._window.InvokeOnMainThread(() =>
            {
                var targetController = (PlaylistViewController)UIStoryboard.FromName("Story", null)
                    .InstantiateViewController("PlaylistViewController");
                targetController.Playlist = dto;
                targetController.ItemUpdated += eventHandler;
                this.Present(targetController, view);
            });*/
        }

        public void SwapToAuth()
        {
            this._window.InvokeOnMainThread(() =>
            {
                var initialController = UIStoryboard.FromName("Auth", null).InstantiateInitialViewController();
                this._window.SwapController(initialController);
            });
        }

        private void Present(UIViewController targetController, IView view, bool push = true)
        {
            var controller = (UIViewController)view;

            if (controller.NavigationController != null && push)
            {
                controller.NavigationController.PushViewController(targetController, true);
            }
            else
            {
                controller.PresentViewController(targetController, true, null);
            }
        }
    }
}