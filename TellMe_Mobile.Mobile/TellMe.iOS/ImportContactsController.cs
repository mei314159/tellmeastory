using System;
using TellMe.Core;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Core.Providers;
using UIKit;

namespace TellMe.iOS
{
    public partial class ImportContactsController : UIViewController, IImportContactsView
    {
        private ImportContactsBusinessLogic _businessLogic;
        public ImportContactsController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _businessLogic = new ImportContactsBusinessLogic(App.Instance.Router, new ContactsProvider(), new RemoteContactsDataService(), this);
        }

        async partial void ProvideAccessButton_TouchUpInside(UIButton sender)
        {
            await _businessLogic.SynchronizeContacts();
        }

        public void ShowErrorMessage(string title, string message = null)
        {
            InvokeOnMainThread(() =>
            {
                UIAlertController alert = UIAlertController
                    .Create(title,
                            message ?? string.Empty,
                            UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                this.PresentViewController(alert, true, null);
            });
        }
    }
}