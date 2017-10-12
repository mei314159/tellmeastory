using System;
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

		public void NavigateRequestStory(IView view, RequestStoryEventHandler e)
		{
			this.window.InvokeOnMainThread(() =>
			{
                var targetController = (UINavigationController)UIStoryboard.FromName("Story", null).InstantiateViewController("RequestStoryViewController");
                ((RequestStoryViewController)targetController.ViewControllers.First()).StoryRequested += e;
				var controller = (UIViewController)view;
				controller.PresentViewController(targetController, true, null);
			});
		}

        public void NavigateRecordStory(IView view, StoryDTO requestedStory = null)
        {
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = (UINavigationController)UIStoryboard.FromName("Story", null).InstantiateViewController("RecordVideoController");
                ((RecordVideoController)targetController.ViewControllers.First()).RequestedStory = requestedStory;
				var controller = (UIViewController)view;
				controller.PresentViewController(targetController, true, null);
			});
        }

        public void NavigatePreviewStory(IView view, string videoPath, StoryDTO requestedStory = null)
		{
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = (PreviewVideoController)UIStoryboard.FromName("Story", null).InstantiateViewController("PreviewVideoController");
                targetController.VideoPath = videoPath;
                targetController.RequestedStory = requestedStory;
				this.Present(targetController, view);
			});
		}

		public void NavigateStoryDetails(IView view, string videoPath, string previewImagePath, StoryDTO requestedStory = null)
		{
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = (SendStoryDetailsViewController)UIStoryboard.FromName("Story", null).InstantiateViewController("SendStoryDetailsViewController");
				targetController.VideoPath = videoPath;
                targetController.PreviewImagePath = previewImagePath;
                targetController.RequestedStory = requestedStory;
				this.Present(targetController, view);
			});
		}

		public void NavigateChooseRecipients(IView view, ContactsSelectedEventHandler e)
		{
			//this.window.InvokeOnMainThread(() =>
			//{
   //             var targetController = (ContactsViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("ContactsViewController");
   //             targetController.ContactsSelected += e;
			//	this.Present(targetController, view);
			//});
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

        private void Present(UIViewController targetController, IView view)
        {
            var controller = (UIViewController)view;
            if (controller.NavigationController != null)
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
