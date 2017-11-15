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

namespace TellMe.iOS
{
    public partial class TribeViewController : UITableViewController, ITribeView
    {
        private ITribeViewBusinessLogic _businessLogic;
        private volatile bool _loadingMore;
        private volatile bool _canLoadMore;

        public event TribeLeftHandler TribeLeft;

        private readonly List<StoryDTO> _storiesList = new List<StoryDTO>();
        public TribeDTO Tribe { get; set; }
        public int TribeId { get; set; }

        public TribeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;
            this._businessLogic = IoC.Container.GetInstance<ITribeViewBusinessLogic>();
            _businessLogic.View = this;
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.TableView.AllowsSelection = false;
            this.NavigationController.View.BackgroundColor = UIColor.White;

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

        public void DisplayTribe(TribeDTO tribe)
        {
            InvokeOnMainThread(() =>
            {
                //NavItem.Title = tribe.Name;
                this.TribeName.Text = tribe.Name;
            });
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection)_storiesList).SyncRoot)
            {
                var initialCount = _storiesList.Count;
                _storiesList.Clear();
                _storiesList.AddRange(stories);

                this._canLoadMore = stories.Count > initialCount;
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);
        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);

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

        private void Cell_ReceiverTouched(StoryReceiverDTO receiver, StoriesListCell cell)
        {
            _businessLogic.ViewReceiver(receiver, (tribe) =>
            {
                cell.RemoveTribe(tribe);
                if (tribe.Id == Tribe.Id)
                {
                    TribeLeft?.Invoke(tribe);
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

        partial void SendStoryTouched(NSObject sender)
        {
            _businessLogic.SendStory();
        }

        partial void InfoButtonTouched(NSObject sender)
        {
            _businessLogic.TribeInfo();
        }

        void ITribeView.TribeLeft(TribeDTO tribe)
        {
            TribeLeft?.Invoke(tribe);
        }
    }
}
