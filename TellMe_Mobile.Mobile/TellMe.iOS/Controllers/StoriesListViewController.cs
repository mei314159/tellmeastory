using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Badge;
using TellMe.iOS.Views.Cells;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class StoriesListViewController : UITableViewController, IStoriesListView
    {
        private IStoriesBusinessLogic _businessLogic;
        private readonly List<StoryDTO> _storiesList = new List<StoryDTO>();
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;
        private BadgeView _notificationsBadge;

        public StoriesListViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;
            this._businessLogic = IoC.GetInstance<IStoriesBusinessLogic>();
            _businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
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

            ((AppDelegate) UIApplication.SharedApplication.Delegate).CheckPushNotificationsPermissions();
        }

        public override void ViewWillAppear(bool animated)
        {
            _businessLogic.LoadActiveNotificationsCountAsync();
            this.SetToolbarItems(new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem("Request a Story", UIBarButtonItemStyle.Plain, RequestStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace),
                new UIBarButtonItem("Send a Story", UIBarButtonItemStyle.Plain, SendStoryButtonTouched),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            }, true);
            this.NavigationController.SetToolbarHidden(false, true);
        }

        public override void ViewDidAppear(bool animated)
        {
            //_businessLogic.NavigateEvents();
        }

        [Action("UnwindToStoriesViewController:")]
        public void UnwindToStoriesViewController(UIStoryboardSegue segue)
        {
            Task.Run(() => LoadStoriesAsync(true, false));
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

        private async void Cell_LikeButtonTouched(StoryDTO story)
        {
            await _businessLogic.LikeButtonTouchedAsync(story).ConfigureAwait(false);
        }

        private void Cell_PreviewTouched(StoryDTO story)
        {
            _businessLogic.ViewStory(story);
        }

        private void Cell_ProfilePictureTouched(StoryDTO story)
        {
            _businessLogic.NavigateStoryteller(story);
        }

        private void Cell_CommentsButtonTouched(StoryDTO story)
        {
            _businessLogic.ViewStory(story, true);
        }

        private void Cell_ReceiverTouched(StoryReceiverDTO receiver, StoriesListCell cell)
        {
            _businessLogic.ViewReceiver(receiver, cell.RemoveTribe);
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_storiesList.Count - indexPath.Row == 5 && _canLoadMore)
            {
                LoadMoreAsync();
            }
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
            await _businessLogic.LoadStoriesAsync();
            InvokeOnMainThread(() =>
            {
                this.ActivityIndicator.StopAnimating();
                this.TableView.TableFooterView.Hidden = true;
            });

            this._loadingMore = false;
        }

        private async Task LoadStoriesAsync(bool forceRefresh, bool clearCache = false)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadStoriesAsync(forceRefresh, clearCache);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
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

        partial void AccountSettingsButton_Activated(UIBarButtonItem sender)
        {
            _businessLogic.AccountSettings();
        }

        partial void Notifications_Activated(UIBarButtonItem sender)
        {
            _businessLogic.NotificationsCenter();
        }

        partial void Storytellers_Activated(UIBarButtonItem sender)
        {
            _businessLogic.ShowStorytellers();
        }

        partial void Events_Activated(UIBarButtonItem sender)
        {
            _businessLogic.NavigateEvents();
        }

        public void DisplayNotificationsCount(int count)
        {
            InvokeOnMainThread(() =>
            {
                if (_notificationsBadge == null)
                {
                    _notificationsBadge = new BadgeView(new CGRect(12, -8, 30, 20));
                    _notificationsBadge.Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular);
                    _notificationsBadge.Hidden = true;

                    Notifications.CustomView = new UIImageView(UIImage.FromBundle("Bell"))
                    {
                        Frame = new CGRect(0, 0, 24, 24),
                    };
                    Notifications.CustomView.Add(_notificationsBadge);
                    Notifications.CustomView.AddGestureRecognizer(
                        new UITapGestureRecognizer(() => Notifications_Activated(Notifications)));
                }

                _notificationsBadge.Hidden = count == 0;
                _notificationsBadge.Value = count;
            });
        }

        public void OnStoryLikeChanged(StoryDTO story)
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
    }
}