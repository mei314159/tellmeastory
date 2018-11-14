using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public class StoriesTableViewController : UITableViewController, IStoriesTableView
    {
        private readonly List<StoryDTO> _itemsList = new List<StoryDTO>();
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;
        private UIImageView _noItemsBackground;

        protected IStoriesTableBusinessLogic BusinessLogic { get; set; }

        public StoriesTableViewController(IntPtr handle) : base(handle)
        {
        }

        public virtual UIActivityIndicatorView ActivityIndicator { get; set; }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            _noItemsBackground = new UIImageView(UIImage.FromBundle("NoStories"))
            {
                ContentMode = UIViewContentMode.Center
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var image = UIImage.FromBundle("NoStories");
            BusinessLogic.View = this;
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;
            App.Instance.OnStoryObjectionableChanged += OnStoryObjectionableChanged;
            this.RefreshControl = new UIRefreshControl();
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.TableView.AllowsSelection = false;
            SetTableBackground();
            this.NavigationController.View.BackgroundColor = UIColor.White;
            //Task.Run(() => LoadStoriesAsync(false, true));

            var overlay = new Overlay("Loading");
            overlay.PopUp(false);
            Task.Run(async () =>
            {
                var result = await this.BusinessLogic.InitAsync();
                overlay.Close();
                if (result)
                {
                    LoadStoriesAsync(true);
                }
            });
        }

        private void OnStoryObjectionableChanged(int storyId, bool objectionable)
        {
            InvokeOnMainThread(() =>
            {
                var index = _itemsList.IndexOf(x => x.Id == storyId);
                if (index <= -1) return;

                var cell = (StoriesListCell) this.TableView.CellAt(NSIndexPath.FromRowSection(index, 0));
                cell.UpdateObjectionableState(objectionable);
            });
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_itemsList.Count - indexPath.Row - StoryItemIndexOffset == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        public virtual int StoryItemIndexOffset => 0;

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._itemsList.Count + StoryItemIndexOffset;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (StoriesListCell) tableView.DequeueReusableCell(StoriesListCell.Key, indexPath);
            cell.Story = this._itemsList[indexPath.Row - StoryItemIndexOffset];
            cell.ProfilePictureTouched = Cell_ProfilePictureTouched;
            cell.PreviewTouched = Cell_PreviewTouched;
            cell.CommentsButtonTouched = Cell_CommentsButtonTouched;
            cell.LikeButtonTouched = Cell_LikeButtonTouched;
            cell.ReceiverSelected = Cell_ReceiverTouched;
            cell.MoreButtonTouched = Cell_MoreButtonTouched;
            cell.UserInteractionEnabled = true;
            return cell;
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);


        public IOverlay DisableInput()
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp();

            return overlay;
        }

        public void EnableInput(IOverlay overlay)
        {
            overlay?.Close(false);
            overlay?.Dispose();
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection) _itemsList).SyncRoot)
            {
                var initialCount = _itemsList.Count;
                _itemsList.Clear();
                _itemsList.AddRange(stories);

                this._canLoadMore = stories.Count > initialCount;
            }

            InvokeOnMainThread(() =>
            {
                SetTableBackground();
                TableView.ReloadData();
            });
        }

        private async void Cell_LikeButtonTouched(StoryDTO story)
        {
            await BusinessLogic.LikeButtonTouchedAsync(story).ConfigureAwait(false);
        }

        private void Cell_PreviewTouched(StoryDTO story)
        {
            BusinessLogic.ViewStory(story);
        }

        private void Cell_ProfilePictureTouched(StoryDTO story)
        {
            BusinessLogic.NavigateStoryteller(story.SenderId);
        }

        private void Cell_CommentsButtonTouched(StoryDTO story)
        {
            BusinessLogic.ViewStory(story, true);
        }

        private void Cell_ReceiverTouched(StoryReceiverDTO receiver, StoriesListCell cell)
        {
            BusinessLogic.NavigateReceiver(receiver, cell.RemoveTribe);
        }

        private void Cell_MoreButtonTouched(StoryDTO story)
        {
            var uiAlertController = UIAlertController.Create("Options", null, UIAlertControllerStyle.ActionSheet);

            if (story.Objectionable)
            {
                uiAlertController.AddAction(UIAlertAction.Create("Unflag as objectionable",
                    UIAlertActionStyle.Destructive,
                    obj => { InvokeInBackground(async () => await BusinessLogic.UnflagAsObjectionable(story.Id)); }));
            }
            else
            {
                uiAlertController.AddAction(UIAlertAction.Create("Add to Playlist", UIAlertActionStyle.Default,
                    (obj) => BusinessLogic.AddToPlaylist(story)));
                uiAlertController.AddAction(UIAlertAction.Create("Flag as objectionable",
                    UIAlertActionStyle.Destructive,
                    obj => { InvokeInBackground(async () => await BusinessLogic.FlagAsObjectionable(story.Id)); }));
            }

            uiAlertController.AddAction(UIAlertAction.Create("Unfollow StoryTeller", UIAlertActionStyle.Destructive,
                (obj) =>
                {
                    InvokeInBackground(async () => await BusinessLogic.UnfollowStoryTellerAsync(story.SenderId));
                }));

            uiAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            this.PresentViewController(uiAlertController, true, null);
        }

        private async Task LoadMoreAsync()
        {
            if (this._loadingMore)
                return;

            this._loadingMore = true;
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StartAnimating();
                this.TableView.TableFooterView.Hidden = false;
            });
            await BusinessLogic.LoadStoriesAsync();
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this._loadingMore = false;
        }

        protected async Task LoadStoriesAsync(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.RefreshControl.BeginRefreshing());
            await BusinessLogic.LoadStoriesAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.RefreshControl.EndRefreshing());
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStoriesAsync(true));
        }

        private void OnStoryLikeChanged(StoryDTO story)
        {
            InvokeOnMainThread(() =>
            {
                var index = _itemsList.IndexOf(x => x.Id == story.Id);
                if (index <= -1) return;

                var cell = (StoriesListCell) this.TableView.CellAt(NSIndexPath.FromRowSection(index, 0));
                cell.UpdateLikeButton(story);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ActivityIndicator?.Dispose();
                ActivityIndicator = null;
            }

            base.Dispose(disposing);
        }

        private void SetTableBackground()
        {
            this.TableView.BackgroundView = this._itemsList.Count > 0 ? null : _noItemsBackground;
        }
    }
}