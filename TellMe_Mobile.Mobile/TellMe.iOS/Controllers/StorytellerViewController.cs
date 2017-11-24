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
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class StorytellerViewController : UITableViewController, IStorytellerView
    {
        private IStorytellerBusinessLogic _businessLogic;
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;

        private readonly List<StoryDTO> _storiesList = new List<StoryDTO>();
        public StorytellerDTO Storyteller { get; set; }
        public string StorytellerId { get; set; }

        private UIImage _defaultPicture;

        public StorytellerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;
            this._businessLogic = IoC.GetInstance<IStorytellerBusinessLogic>();
            _businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.NavigationController.View.BackgroundColor = UIColor.White;
            this.TableView.AllowsSelection = false;
            this._defaultPicture = UIImage.FromBundle("UserPic");


            var overlay = new Overlay("Loading");
            overlay.PopUp(false);
            Task.Run(async () =>
            {
                var result = await this._businessLogic.InitAsync();
                overlay.Close(true);
                if (result)
                {
                    LoadStoriesAsync(true);
                }
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, false);
        }

        [Action("UnwindToStoriesViewController:")]
        public void UnwindToStoriesViewController(UIStoryboardSegue segue)
        {
            Task.Run(() => LoadStoriesAsync(true));
        }

        public void DisplayStoryteller(StorytellerDTO storyteller)
        {
            InvokeOnMainThread(() =>
            {
                NavItem.Title = storyteller.UserName;
                this.UserName.Text = storyteller.UserName;
                this.FullName.Text = storyteller.FullName;
                this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl, _defaultPicture);
            });
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

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this._storiesList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this._storiesList[indexPath.Row];
            cell.ProfilePictureTouched = Cell_ProfilePictureTouched;
            cell.PreviewTouched = Cell_PreviewTouched;
            cell.ReceiverSelected = Cell_ReceiverTouched;
            cell.LikeButtonTouched = Cell_LikeButtonTouched;
            cell.UserInteractionEnabled = true;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_storiesList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
        }

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
        
        private async void Cell_LikeButtonTouched(StoryDTO story)
        {
            await _businessLogic.LikeButtonTouchedAsync(story).ConfigureAwait(false);
        }

        private void Cell_ReceiverTouched(StoryReceiverDTO receiver, StoriesListCell cell)
        {
            _businessLogic.ViewReceiver(receiver, cell.RemoveTribe);
        }

        private void Cell_PreviewTouched(StoryDTO story)
        {
            _businessLogic.ViewStory(story);
        }

        private void Cell_ProfilePictureTouched(StoryDTO story)
        {
            _businessLogic.NavigateStoryteller(story);
        }

        private async Task LoadMoreAsync()
        {
            if (this._loadingMore)
                return;

            this._loadingMore = true;
            InvokeOnMainThread(() =>
            {
                this.Spinner.StartAnimating();
                this.TableView.TableFooterView.Hidden = false;
            });
            await _businessLogic.LoadStoriesAsync(false);
            InvokeOnMainThread(() =>
            {
                this.Spinner.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this._loadingMore = false;
        }

        private async Task LoadStoriesAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadStoriesAsync(forceRefresh).ConfigureAwait(false);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        private void OnStoryLikeChanged(StoryDTO story)
        {
            InvokeOnMainThread(() =>
            {
                var index = _storiesList.IndexOf(x => x.Id == story.Id);
                if (index > -1)
                {
                    var cell = TableView.CellAt(NSIndexPath.FromRowSection(index, 0)) as StoriesListCell;
                    cell?.UpdateLikeButton(story);
                }
            });
        }

        private void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStoriesAsync(true));
        }

        private void SendStoryButtonTouched(object sender, EventArgs e)
        {
            _businessLogic.SendStory();
        }

        private void RequestStoryButtonTouched(object sender, EventArgs e)
        {
            _businessLogic.RequestStory();
        }

        partial void RequestStoryTouched(NSObject sender)
        {
            _businessLogic.RequestStory();
        }

        partial void SendStoryTouched(NSObject sender)
        {
            _businessLogic.SendStory();
        }
    }
}