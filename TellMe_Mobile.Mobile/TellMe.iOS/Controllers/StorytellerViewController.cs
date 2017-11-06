using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using UIKit;

namespace TellMe.iOS
{
    public partial class StorytellerViewController : UITableViewController, IStorytellerView
    {
        private StorytellerBusinessLogic _businessLogic;
        private volatile bool loadingMore;
        private volatile bool canLoadMore;

        public List<StoryDTO> storiesList = new List<StoryDTO>();
        public StorytellerDTO Storyteller { get; set; }
        public string StorytellerId { get; set; }

        UIImage defaultPicture;

        public StorytellerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = new StorytellerBusinessLogic(new RemoteStoriesDataService(), new RemoteStorytellersDataService(), this, App.Instance.Router);
            this.TableView.RegisterNibForCellReuse(StoriesListCell.Nib, StoriesListCell.Key);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 64;
            this.TableView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView = new UIView();
            this.TableView.DelaysContentTouches = false;
            this.TableView.TableFooterView.Hidden = true;
            this.NavigationController.View.BackgroundColor = UIColor.White;
            this.TableView.AllowsSelection = false;
            this.defaultPicture = UIImage.FromBundle("UserPic");


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
                this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl, defaultPicture);
            });
        }

        public void DisplayStories(ICollection<StoryDTO> stories)
        {
            lock (((ICollection)storiesList).SyncRoot)
            {
                var initialCount = storiesList.Count;
                storiesList.Clear();
                storiesList.AddRange(stories);

                this.canLoadMore = stories.Count > initialCount;
            }

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);
        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return this.storiesList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(StoriesListCell.Key, indexPath) as StoriesListCell;
            cell.Story = this.storiesList[indexPath.Row];
            //cell.ProfilePictureTouched = Cell_OnProfilePictureTouched;
            cell.PreviewTouched = Cell_OnPreviewTouched;
            cell.UserInteractionEnabled = true;
            return cell;
        }

        void Cell_OnPreviewTouched(StoryDTO story)
        {
            _businessLogic.ViewStory(story);
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (storiesList.Count - indexPath.Row == 5 && canLoadMore)
            {
                LoadMoreAsync();
            }
        }

        private async Task LoadMoreAsync()
        {
            if (this.loadingMore)
                return;

            this.loadingMore = true;
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

            this.loadingMore = false;
        }

        private async Task LoadStoriesAsync(bool forceRefresh)
        {
            InvokeOnMainThread(() => this.TableView.RefreshControl.BeginRefreshing());
            await _businessLogic.LoadStoriesAsync(forceRefresh).ConfigureAwait(false);
            InvokeOnMainThread(() => this.TableView.RefreshControl.EndRefreshing());
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            Task.Run(() => LoadStoriesAsync(true));
        }

        void SendStoryButtonTouched(object sender, EventArgs e)
        {
            _businessLogic.SendStory();
        }

        void RequestStoryButtonTouched(object sender, EventArgs e)
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
