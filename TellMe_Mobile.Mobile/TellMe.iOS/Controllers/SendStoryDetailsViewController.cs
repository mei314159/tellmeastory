using Foundation;
using System;
using UIKit;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core;
using TellMe.Core.Contracts.UI.Views;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using System.Collections;
using TellMe.iOS.Views.Cells;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class SendStoryDetailsViewController : UIViewController, ISendStoryDetailsView, IUITableViewDataSource
    {
        SendStoryDetailsBusinessLogic _businessLogic;
        private List<StorytellerDTO> recipientsList;

        public SendStoryDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public string VideoPath { get; set; }

        ITextInput ISendStoryDetailsView.StoryName => this.StoryName;
        IButton ISendStoryDetailsView.SendButton => this.SendButton;
        IButton ISendStoryDetailsView.ChooseRecipientsButton => this.ChooseRecipientsButton;

        public string PreviewImagePath { get; set; }
        public StoryDTO RequestedStory { get; set; }

        public override void ViewDidLoad()
        {
            recipientsList = new List<StorytellerDTO>();
            _businessLogic = new SendStoryDetailsBusinessLogic(this, App.Instance.Router, new RemoteStoriesDataService());
            //this.RecipientsTable.RegisterNibForCellReuse(ContactsListCell.Nib, ContactsListCell.Key);
            this.RecipientsTable.RowHeight = 64;
            this.RecipientsTable.DataSource = this;
            this.RecipientsTable.TableFooterView = new UIView();
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            this._businessLogic.Init();
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

        public void DisplayRecipients(ICollection<StorytellerDTO> recipients)
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
            return new UITableViewCell();
            //var cell = tableView.DequeueReusableCell(ContactsListCell.Key, indexPath) as ContactsListCell;
            //cell.Contact = this.recipientsList[indexPath.Row];
            //return cell;
        }

        async partial void SendButton_TouchUpInside(Button sender)
        {
            await _businessLogic.SendAsync();
        }

        partial void ChooseRecipientsButton_TouchUpInside(Button sender)
        {
            _businessLogic.ChooseRecipients();
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

        public void Close()
        {
            this.DismissViewController(true, null);
        }
    }
}