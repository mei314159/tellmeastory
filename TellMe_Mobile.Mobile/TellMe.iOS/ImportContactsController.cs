using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using Contacts;
using TellMe.Core.DTO;
using System.Linq;
using TellMe.Core;
using TellMe.Core.Services;
using TellMe.iOS.Extensions;
using TellMe.Core.Contracts.Providers;
using TellMe.iOS.Core.Providers;

namespace TellMe.iOS
{
    public partial class ImportContactsController : UIViewController
    {
        private ContactsService contactsService;
		private IContactsProvider contactsProvider;

		public ImportContactsController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.contactsService = new ContactsService(App.Instance.DataStorage);
            this.contactsProvider = new ContactsProvider();
        }

        async partial void ProvideAccessButton_TouchUpInside(UIButton sender)
        {
            var status = contactsProvider.GetPermissions();
            if (status == Permissions.Denied
            || status == Permissions.Restricted)
            {
                UIAlertController alert = UIAlertController
                .Create("Access denied",
                        "Please provide an access to your contacts list manually in Settings > Privacy > Contacts",
                        UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (obj) =>
                {
                }));
                this.PresentViewController(alert, true, null);
                return;
            }

            var contacts = contactsProvider.GetContacts();

            var syncResult = await contactsService.SynchronizeContactsAsync(contacts);
            if (syncResult.IsValid)
            {
                this.View.Window.SwapController(UIStoryboard.FromName("Main", null).InstantiateInitialViewController());
            }
            else{
				UIAlertController alert = UIAlertController
				.Create("Error",
                        syncResult.ErrorsString,
						UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (obj) =>
				{
				}));

                this.PresentViewController(alert, true, null);
            }
        }
    }
}