using System;
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

        public void NavigateImportContacts()
        {
            this.window.InvokeOnMainThread(() => this.window.SwapController(UIStoryboard.FromName("Auth", null).InstantiateViewController("ImportContactsController")));
        }

        public void NavigateMain()
        {
            this.window.InvokeOnMainThread(() => this.window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController()));
        }

        public void NavigateContactDetails(IView view, ContactDTO dto)
        {
            this.window.InvokeOnMainThread(() =>
            {

                var targetController = (ContactDetailsViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("ContactDetailsViewController");
                targetController.ContactDTO = dto;
				this.Present(targetController, view);
            });
        }


		public void NavigateRequestStory(IView view)
		{
			//this.window.InvokeOnMainThread(() =>
			//{
   //             var targetController = new RequestStoryViewController();
			//	this.Present(targetController, view);
			//});
		}

        public void NavigateRecordStory(IView view)
        {
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = UIStoryboard.FromName("Story", null).InstantiateViewController("RecordVideoController");
				var controller = (UIViewController)view;
				controller.PresentViewController(targetController, true, null);
			});
        }

        public void NavigatePreviewStory(IView view, string videoPath)
		{
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = (PreviewVideoController)UIStoryboard.FromName("Story", null).InstantiateViewController("PreviewVideoController");
                targetController.VideoPath = videoPath;
				this.Present(targetController, view);
			});
		}



		public void NavigateStoryDetails(IView view, string videoPath, string previewImagePath)
		{
			this.window.InvokeOnMainThread(() =>
			{
				var targetController = (SendStoryDetailsViewController)UIStoryboard.FromName("Story", null).InstantiateViewController("SendStoryDetailsViewController");
				targetController.VideoPath = videoPath;
                targetController.PreviewImagePath = previewImagePath;
				this.Present(targetController, view);
			});
		}

		public void NavigateChooseRecipients(IView view, ContactsSelectedEventHandler e)
		{
			this.window.InvokeOnMainThread(() =>
			{
                var targetController = (ContactsViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("ContactsViewController");
                targetController.ContactsSelected += e;
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
