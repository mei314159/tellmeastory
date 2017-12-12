using Foundation;
using UIKit;
using MonoTouch.Dialog;
using TellMe.iOS.Views.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using TellMe.iOS.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.iOS.Controllers
{
    public class CreateTribeController : DialogViewController, ICreateTribeView
    {
        private readonly ICreateTribeBusinessLogic _businessLogic;
        private EntryElement _tribeNameElement;
        public ICollection<StorytellerDTO> Members { get; private set; }

        public string TribeName => _tribeNameElement?.Value;

        public event TribeCreatedHandler TribeCreated;

        public CreateTribeController(ICollection<StorytellerDTO> members) : base(UITableViewStyle.Grouped, null, true)
        {
            this.Members = members;
            _businessLogic = IoC.GetInstance<ICreateTribeBusinessLogic>();
            _businessLogic.View = this;
        }

        public override void ViewDidLoad()
        {
            this.NavigationItem.RightBarButtonItem =
                new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, DoneButtonTouched);

            _tribeNameElement = new EntryElement("Name", "Enter tribe name", string.Empty);
            this.Root = new RootElement("New Tribe")
            {
                new Section(string.Empty)
                {
                    _tribeNameElement
                },
                new Section("Members")
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

        private async void DoneButtonTouched(object sender, EventArgs e)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();
            await _businessLogic.CreateTribeAsync();
            overlay.Close();
        }

        public void Close(TribeDTO tribe)
        {
            TribeCreated?.Invoke(tribe);
            var index = this.NavigationController.ViewControllers.ToList().IndexOf(this);
            this.NavigationController.PopToViewController(this.NavigationController.ViewControllers[index - 2], true);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);
    }

    public class CreateTribeSource : DialogViewController.Source
    {
        private readonly ICollection<StorytellerDTO> _members;

        public CreateTribeSource(DialogViewController controller, ICollection<StorytellerDTO> members) : base(
            controller)
        {
            this._members = members;
            controller.TableView.RegisterNibForCellReuse(StorytellersListCell.Nib, StorytellersListCell.Key);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return section == 0 ? base.RowsInSection(tableview, section) : _members.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
                return base.GetCell(tableView, indexPath);

            var cell = (StorytellersListCell) tableView.DequeueReusableCell(StorytellersListCell.Key, indexPath);
            cell.Storyteller = _members.ElementAt(indexPath.Row);
            return cell;
        }
    }
}