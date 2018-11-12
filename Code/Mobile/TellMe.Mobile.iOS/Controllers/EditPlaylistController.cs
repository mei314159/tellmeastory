using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MonoTouch.Dialog;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Shared.Contracts.DTO.Interfaces;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public class EditPlaylistController : DialogViewController, ICreatePlaylistView
    {
        private readonly IEditPlaylistBusinessLogic _businessLogic;
        private CreatePlaylistSource _dataSource;
        private bool _createItem;

        public EditPlaylistController(IEditPlaylistBusinessLogic businessLogic) : base(UITableViewStyle.Grouped, null,
            true)
        {
            _businessLogic = businessLogic;
            _businessLogic.View = this;
        }

        public event ItemUpdateHandler<PlaylistDTO> ItemUpdated;

        public PlaylistDTO Playlist { get; set; }
        public List<IStoryDTO> Stories { get; set; }

        public override async void ViewDidLoad()
        {
            Stories = new List<IStoryDTO>();
            this._createItem = Playlist == null;
            if (Playlist == null)
            {
                Playlist = new PlaylistDTO();
            }
            else
            {
                Stories.AddRange(await _businessLogic.LoadStoriesAsync(Playlist.Id));
            }

            ToggleRightButton(true);
            TableView.RefreshControl = new UIRefreshControl();
            TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.AllowsSelectionDuringEditing = true;
            TableView.SetEditing(true, true);
            this.Root = new RootElement("Edit Playlist")
            {
                new Section("Playlist Info")
                {
                    new EntryElement("Title", "Playlist Title", Playlist?.Name),
                },

                new Section("Stories")
            };

            if (!_createItem)
                LoadAsync(false);
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
            _dataSource = new CreatePlaylistSource(this);
            _dataSource.SetData(Stories);
            _dataSource.EditButtonTouched += DataSource_EditButtonTouched;
            _dataSource.OnDeleteRow += DataSource_OnDeleteRow;
            _dataSource.OnItemSelected += DataSourceOnItemSelected;
            return _dataSource;
        }

        private async Task LoadAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadAsync(forceRefresh);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        private void ToggleRightButton(bool showButton)
        {
            InvokeOnMainThread(() =>
            {
                this.NavigationItem.RightBarButtonItem = showButton
                    ? new UIBarButtonItem(_createItem ? "Continue" : "Save", UIBarButtonItemStyle.Done,
                        ContinueButtonTouched)
                    : null;
            });
        }

        private void DataSource_EditButtonTouched()
        {
            _businessLogic.ChooseStories();
        }

        private void DataSource_OnDeleteRow(IStoryDTO deletedItem, NSIndexPath indexPath)
        {
            Stories.RemoveAll(x => x.Id == deletedItem.Id);
            TableView.DeleteRows(new[] {indexPath}, UITableViewRowAnimation.Automatic);
        }

        private void DataSourceOnItemSelected(IStoryDTO storyDTO, NSIndexPath indexPath)
        {
            _businessLogic.NavigateStory(storyDTO.Id);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            LoadAsync(true);
        }

        public void Display(PlaylistDTO dto)
        {
            this.Playlist = dto;
            InvokeOnMainThread(() =>
            {
                var root = this.Root[0];
                ((EntryElement) root[0]).Value = dto.Name;
                if (this.Root.Count == 2)
                {
                    var deleteButton = new UIButton(UIButtonType.System);
                    deleteButton.SetTitle("Delete Playlist", UIControlState.Normal);
                    deleteButton.TouchUpInside += DeleteButton_TouchUpInside;
                    deleteButton.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44);
                    deleteButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
                    Root.Add(new Section(string.Empty)
                    {
                        new UIViewElement(string.Empty, deleteButton, false)
                    });
                }

                DisplayStories();
            });
        }

        private void DeleteButton_TouchUpInside(object sender, EventArgs e)
        {
            var alert = UIAlertController.Create(
                "Delete a playlist",
                "Do you really want to delete this playlist?",
                UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("Yes, I do", UIAlertActionStyle.Default,
                async obj => { await _businessLogic.DeletePlaylistAsync(); }));
            this.PresentViewController(alert, true, null);
        }

        public void DisplayStories()
        {
            _dataSource.SetData(Stories);
            TableView.ReloadData();
        }

        private void ContinueButtonTouched(object sender, EventArgs e)
        {
            this.HideKeyboard();
            var root = this.Root[0];
            TableView.RefreshControl.Enabled = true;
            Playlist.Name = ((EntryElement) root[0]).Value;
            _businessLogic.SaveAsync();
        }

        public void Deleted(PlaylistDTO dto)
        {
            ItemUpdated?.Invoke(dto, ItemState.Deleted);
            ((IDismissable) this).Dismiss();
        }

        public void Saved(PlaylistDTO dto)
        {
            ItemUpdated?.Invoke(dto, ItemState.Updated);
            ((IDismissable) this).Dismiss();
        }

        void IDismissable.Dismiss()
        {
            InvokeOnMainThread(() =>
            {
                if (NavigationController != null)
                    this.NavigationController.PopViewController(true);
                else
                    this.DismissViewController(true, null);
            });
        }

        public IOverlay DisableInput()
        {
            ToggleRightButton(false);
            var overlay = new Overlay("Wait");
            overlay.PopUp();

            return overlay;
        }

        public void EnableInput(IOverlay overlay)
        {
            ToggleRightButton(true);
            overlay?.Close(false);
            overlay?.Dispose();
        }
    }

    public class CreatePlaylistSource : DialogViewController.Source
    {
        private readonly List<IStoryDTO> _itemsList = new List<IStoryDTO>();
        private UITableViewCell _addMemberCell;

        public event Action<IStoryDTO, NSIndexPath> OnDeleteRow;
        public event Action<IStoryDTO, NSIndexPath> OnItemSelected;
        public event Action EditButtonTouched;

        public CreatePlaylistSource(DialogViewController controller) : base(controller)
        {
            controller.TableView.RegisterNibForCellReuse(SlimStoryCell.Nib,
                SlimStoryCell.Key);
        }

        public void SetData(ICollection<IStoryDTO> stories)
        {
            lock (((ICollection) _itemsList).SyncRoot)
            {
                _itemsList.Clear();
                _itemsList.AddRange(stories);
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section != 1)
                return base.RowsInSection(tableview, section);
            return tableview.Editing ? _itemsList.Count + 1 : _itemsList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.GetCell(tableView, indexPath);

            if (tableView.Editing && indexPath.Row == 0)
            {
                if (_addMemberCell == null)
                {
                    _addMemberCell = new UITableViewCell();
                    _addMemberCell.TextLabel.Text = "Add Stories";
                    _addMemberCell.TextLabel.TextAlignment = UITextAlignment.Center;
                    _addMemberCell.TextLabel.TextColor = _addMemberCell.DefaultTintColor();
                }

                return _addMemberCell;
            }

            var cell = (SlimStoryCell) tableView.DequeueReusableCell(SlimStoryCell.Key,
                indexPath);
            var index = tableView.Editing ? indexPath.Row - 1 : indexPath.Row;
            cell.Story = _itemsList.ElementAt(index);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
            {
                base.RowSelected(tableView, indexPath);
                return;
            }

            if (tableView.Editing && indexPath.Row == 0)
            {
                EditButtonTouched?.Invoke();
            }
            else
            {
                var cell = (SlimStoryCell) tableView.CellAt(indexPath);
                OnItemSelected?.Invoke(cell.Story, indexPath);
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var result = indexPath.Section == 1 && indexPath.Row != 0;
            return result;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section != 1)
                return base.EditingStyleForRow(tableView, indexPath);

            return indexPath.Row == 0 ? UITableViewCellEditingStyle.None : UITableViewCellEditingStyle.Delete;
        }

        public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var deleteAction = UITableViewRowAction.Create(UITableViewRowActionStyle.Destructive, "Delete", DeleteRow);
            return new[] {deleteAction};
        }

        private void DeleteRow(UITableViewRowAction action, NSIndexPath indexPath)
        {
            var deletedItem = _itemsList[(indexPath.Row - 1)];
            _itemsList.Remove(deletedItem);
            OnDeleteRow?.Invoke(deletedItem, indexPath);
        }
    }
}