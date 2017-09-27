using System;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
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

        public void NavigateContactDetails(ContactDTO dto)
        {
            this.window.InvokeOnMainThread(() =>
            {
                var controller = (ContactDetailsViewController)UIStoryboard.FromName("Main", null).InstantiateViewController("ContactDetailsViewController");
                controller.ContactDTO = dto;
                this.window.FindVisibleViewController().PresentViewController(controller, true, null);
            });
        }
    }
}
