using System;
using System.Collections.Generic;
using System.Linq;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using UIKit;

namespace TellMe.iOS.Core
{
    public class Router : IRouter
    {
        private readonly UIWindow window;

        public Router(UIWindow window)
        {
            this.window = window;
        }

        public void NavigateMain()
        {
            this.window.InvokeOnMainThread(() => this.window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController()));
        }

        public void NavigateRequestStory(IView view, StorytellerDTO recipient)
        {
            var targetController = (RequestStoryController)UIStoryboard.FromName("Story", null).InstantiateViewController("RequestStoryController");
            targetController.Recipient = recipient;
            this.Present(targetController, view, true);
        }

        public void NavigateChooseRecipients(IView view, StorytellerSelectedEventHandler e, bool dismissOnFinish)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = (StorytellersController)UIStoryboard.FromName("Story", null).InstantiateViewController("StorytellersController");
                targetController.Mode = StorytellersViewMode.ChooseRecipient;
                targetController.DismissOnFinish = dismissOnFinish;
                targetController.RecipientsSelected += e;
                this.Present(targetController, view);
            });
        }

        public void NavigateRecordStory(IView view, StoryDTO requestedStory = null, NotificationDTO notification = null)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = (UINavigationController)UIStoryboard.FromName("Story", null).InstantiateViewController("RecordVideoController");
                var recordController = (RecordVideoController)targetController.ViewControllers.First();
                recordController.RequestedStory = requestedStory;
                recordController.RequestNotification = notification;
                this.Present(targetController, view, false);
            });
        }

        public void NavigatePreviewStory(IView view, string videoPath, StoryDTO requestedStory = null, NotificationDTO notification = null)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = (PreviewVideoController)UIStoryboard.FromName("Story", null).InstantiateViewController("PreviewVideoController");
                targetController.VideoPath = videoPath;
                targetController.RequestedStory = requestedStory;
                targetController.RequestNotification = notification;
                this.Present(targetController, view);
            });
        }

        public void NavigateStoryDetails(IView view, string videoPath, string previewImagePath, StoryDTO requestedStory = null, NotificationDTO notification = null)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = (SendStoryViewController)UIStoryboard.FromName("Story", null).InstantiateViewController("SendStoryViewController");
                targetController.VideoPath = videoPath;
                targetController.PreviewImagePath = previewImagePath;
                targetController.RequestedStory = requestedStory;
                targetController.RequestNotification = notification;
                this.Present(targetController, view);
            });
        }

        public void NavigateAccountSettings(IView view)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Main", null).InstantiateViewController("ProfileViewController");
                this.Present(targetController, view);
            });
        }

        public void NavigateSetProfilePicture(IView view)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Auth", null).InstantiateViewController("UploadPictureController");
                this.Present(targetController, view);
            });
        }

        public void NavigateStorytellers(IView view)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Story", null).InstantiateViewController("StorytellersController");
                this.Present(targetController, view);
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

        public void NavigateNotificationsCenter(IView view)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var targetController = UIStoryboard.FromName("Main", null).InstantiateViewController("NotificationCenterController");
                this.Present(targetController, view, false);
            });
        }
    }
}
