using Foundation;
using UIKit;
using MonoTouch.Dialog;
using TellMe.iOS.Views.Cells;
using System;
using TellMe.Core.Contracts.DTO;
using System.Collections.Generic;
using System.Linq;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Views;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;

namespace mehspot.iOS
{
    public class CreateTribeController : DialogViewController, ICreateTribeView
    {
        private readonly CreateTribeBusinessLogic _businessLogic;
        private EntryElement tribeNameElement;
        public ICollection<StorytellerDTO> Members { get; private set; }

        public string TribeName => tribeNameElement?.Value;

        public event TribeCreatedHandler TribeCreated;

        public CreateTribeController(ICollection<StorytellerDTO> members) : base(UITableViewStyle.Grouped, null, true)
        {
            this.Members = members;
            _businessLogic = new CreateTribeBusinessLogic(new RemoteTribesDataService(), this);
        }

        public override void ViewDidLoad()
        {
            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, DoneButtonTouched);

            tribeNameElement = new EntryElement("Name", "Enter tribe name", string.Empty);
            this.Root = new RootElement("New Tribe") {
                new Section (string.Empty) {
                    tribeNameElement
                },
                new Section ("Members") {

                }
            };
        }

        public override void Selected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Selected(indexPath);
        }

        public override void Deselected(NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                base.Deselected(indexPath);
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new CreateTribeSource(this, Members);
        }

        async void DoneButtonTouched(object sender, EventArgs e)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.CreateTribeAsync();
            overlay.Close(true);
        }

        public void Close(TribeDTO tribe)
        {
            TribeCreated?.Invoke(tribe);
            var index = this.NavigationController.ViewControllers.ToList().IndexOf(this);
            this.NavigationController.PopToViewController(this.NavigationController.ViewControllers[index - 2], true);
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) => ViewExtensions.ShowSuccessMessage(this, message, complete);
    }

    public class CreateTribeSource : DialogViewController.Source
    {
        private readonly ICollection<StorytellerDTO> members;

        public CreateTribeSource(DialogViewController controller, ICollection<StorytellerDTO> members) : base(controller)
        {
            this.members = members;
            controller.TableView.RegisterNibForCellReuse(StorytellersListCell.Nib, StorytellersListCell.Key);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
                return base.RowsInSection(tableview, section);
            return members.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                return base.GetCell(tableView, indexPath);

            var cell = tableView.DequeueReusableCell(StorytellersListCell.Key, indexPath) as StorytellersListCell;
            cell.Storyteller = members.ElementAt(indexPath.Row);
            return cell;
        }
    }
}