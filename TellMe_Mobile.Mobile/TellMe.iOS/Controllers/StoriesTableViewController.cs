using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public class StoriesTableViewController : UITableViewController, IStoriesTableView
    {
        private readonly List<StoryDTO> _storiesList = new List<StoryDTO>();
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;
        
        protected IStoriesTableBusinessLogic BusinessLogic { get; set; }

        public StoriesTableViewController(IntPtr handle) : base(handle)
        {
        }
        
        public virtual UIActivityIndicatorView ActivityIndicator { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BusinessLogic.View = this;
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;
            this.RefreshControl = new UIRefreshControl();
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.BackgroundColor = UIColor.FromRGB(238, 238, 238);
            this.TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.TableView.AllowsSelection = false;
            this.NavigationController.View.BackgroundColor = UIColor.White;
            Task.Run(() => LoadStoriesAsync(false, true));
            
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

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_storiesList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._storiesList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (StoriesListCell) tableView.DequeueReusableCell(StoriesListCell.Key, indexPath);
            cell.Story = this._storiesList[indexPath.Row];
            cell.ProfilePictureTouched = Cell_ProfilePictureTouched;
            cell.PreviewTouched = Cell_PreviewTouched;
            cell.CommentsButtonTouched = Cell_CommentsButtonTouched;
            cell.LikeButtonTouched = Cell_LikeButtonTouched;
            cell.ReceiverSelected = Cell_ReceiverTouched;
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
            lock (((ICollection) _storiesList).SyncRoot)
            {
                var initialCount = _storiesList.Count;
                _storiesList.Clear();
                _storiesList.AddRange(stories);

                this._canLoadMore = stories.Count > initialCount;
            }

            InvokeOnMainThread(() => this.TableView.ReloadData());
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
            BusinessLogic.NavigateStoryteller(story);
        }

        private void Cell_CommentsButtonTouched(StoryDTO story)
        {
            BusinessLogic.ViewStory(story, true);
        }

        private void Cell_ReceiverTouched(StoryReceiverDTO receiver, StoriesListCell cell)
        {
            BusinessLogic.ViewReceiver(receiver, cell.RemoveTribe);
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
                var index = _storiesList.IndexOf(x => x.Id == story.Id);
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
    }
}