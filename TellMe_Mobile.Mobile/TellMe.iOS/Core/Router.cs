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
				var controller = (UIViewController)view;
                if (controller.NavigationController != null)
                {
                    controller.NavigationController.PushViewController(targetController, true);
                }
                else
                {
                    controller.PresentViewController(controller, true, null);
                }
            });
        }
    }
}
