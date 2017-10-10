using Foundation;
using System;
using UIKit;
using TellMe.Core.Contracts.DTO;
using System.Collections.Generic;
using System.Collections;
using TellMe.iOS.Views.Cells;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class RequestStoryViewController : UIViewController, IRequestStoryView, IUITableViewDataSource
	{
		private RequestStoryBusinessLogic _businessLogic;
		private List<ContactDTO> recipientsList;

		ITextInput IRequestStoryView.StoryName => this.StoryName;
		IButton IRequestStoryView.SendButton => this.SendButton;

        public event RequestStoryEventHandler StoryRequested;

        public RequestStoryViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			recipientsList = new List<ContactDTO>();
			_businessLogic = new RequestStoryBusinessLogic(this, App.Instance.Router, new RemoteStoriesDataService());
			this.RecipientsTable.RegisterNibForCellReuse(ContactsListCell.Nib, ContactsListCell.Key);
			this.RecipientsTable.RowHeight = 64;
			this.RecipientsTable.DataSource = this;
			this.RecipientsTable.TableFooterView = new UIView();
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
		}

		public void DisplayRecipients(ICollection<ContactDTO> recipients)
		{
			lock (((ICollection)recipientsList).SyncRoot)
			{
				recipientsList.Clear();
				recipientsList.AddRange(recipients);
			}

			InvokeOnMainThread(() => RecipientsTable.ReloadData());
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return this.recipientsList.Count;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(ContactsListCell.Key, indexPath) as ContactsListCell;
			cell.Contact = this.recipientsList[indexPath.Row];
			return cell;
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

		public void ShowSuccessMessage(string message, Action complete)
		{
			InvokeOnMainThread(() =>
			{
				UIAlertController alert = UIAlertController
					.Create("Success",
							message ?? string.Empty,
							UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => complete()));
				this.PresentViewController(alert, true, null);
			});
		}

		public void Close(ICollection<StoryDTO> requestedStories)
        {
            this.StoryRequested?.Invoke(requestedStories);
			this.DismissViewController(true, null);
		}

        partial void ChooseRecipientsButton_TouchUpInside(Button sender)
        {
            _businessLogic.ChooseRecipients();
        }

        async partial void SendButton_TouchUpInside(Button sender)
        {
            await _businessLogic.SendAsync();
        }

        partial void CloseButton_Activated(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }
    }
}