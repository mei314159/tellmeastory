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

namespace TellMe.iOS
{
    public partial class ImportContactsController : UIViewController
    {
        private ContactsService contactsService;

        public ImportContactsController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.contactsService = new ContactsService(App.Instance.DataStorage);
        }

        async partial void ProvideAccessButton_TouchUpInside(UIButton sender)
        {
            if (CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts) == CNAuthorizationStatus.Denied
            || CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts) == CNAuthorizationStatus.Restricted)
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

            var contacts = GetAllContacts();

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


        private List<ContactDTO> GetAllContacts()
        {
            var keysTOFetch = new[] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers };

            NSError error;
            CNContact[] contactList;
            var ContainerId = new CNContactStore().DefaultContainerIdentifier;
            using (var predicate = CNContact.GetPredicateForContactsInContainer(ContainerId))
            {
                using (var store = new CNContactStore())
                {
                    contactList = store.GetUnifiedContacts(predicate, keysTOFetch, out error);
                }
            }

            var contacts = new List<ContactDTO>();

            foreach (var item in contactList)
            {
                var numbers = item.PhoneNumbers;
                if (numbers != null)
                {
                    foreach (var number in numbers)
                    {
                        contacts.Add(new ContactDTO
                        {
                            //LocalName = item.GivenName,
                            //LocalId = number.Value.ValueForKey(new NSString("digits")).ToString(),
                            PhoneNumber = number.Value.StringValue
                        });
                    }
                }
            }

            return contacts;
        }
    }
}