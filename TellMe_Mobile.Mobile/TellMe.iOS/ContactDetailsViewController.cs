using System;
using UIKit;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;

namespace TellMe.iOS
{
    public partial class ContactDetailsViewController : UIViewController, IContactDetailsView
    {
        private ContactDetailsBusinessLogic businessLogic;

        public ContactDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public ContactDTO ContactDTO { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.businessLogic = new ContactDetailsBusinessLogic(new RemoteStoriesDataService(), this);
            LoadContactDetails();
        }

        private void LoadContactDetails()
        {
            // TODO Show Spinner
            businessLogic.LoadContactDetails();
            // TODO Hide Spinner
        }

        public void DisplayContactDetails(ContactDTO dto)
        {
            InvokeOnMainThread(() =>
            {
                this.NavItem.Title = dto.Name;
            });
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

        partial void RequestStoryButton_TouchUpInside(UIButton sender)
        {
            businessLogic.RequestStory();
        }

        public void DisplayStoryDetailsPrompt()
        {
            UIAlertController alert = UIAlertController.Create("Request Story", "Please enter title and description", UIAlertControllerStyle.Alert);
            alert.AddTextField(titleField =>
            {
                titleField.Placeholder = "Story Title";
            });
            alert.AddTextField(titleField =>
            {
                titleField.Placeholder = "Story Description";
            });

            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alertAction => RequestStoryPromptSuccess(alert, alertAction)));

            this.PresentViewController(alert, true, null);
        }

        private async void RequestStoryPromptSuccess(UIAlertController alert, UIAlertAction alertAction)
        {
            var title = alert.TextFields[0].Text;
            var description = alert.TextFields[1].Text;

            await this.businessLogic.RequestStoryAsync(title, description);
        }

        public void ShowSuccessMessage(string message)
        {
			InvokeOnMainThread(() =>
			{
				UIAlertController alert = UIAlertController
					.Create("Success",
							message ?? string.Empty,
							UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				this.PresentViewController(alert, true, null);
			});
        }
    }
}