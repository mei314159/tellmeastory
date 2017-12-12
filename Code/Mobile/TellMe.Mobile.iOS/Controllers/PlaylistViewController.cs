using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using ObjCRuntime;
using SDWebImage;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Shared.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class PlaylistViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource,
        IAVAssetDownloadDelegate
    {
        private AVPlayer _player;
        private AVPlayerLayer _playerLayer;
        private NSObject _stopPlayingNotification;
        private bool _playing;
        private int _currentItemIndex;
        private bool _playerVisible = true;

        private static readonly NSString AvCustomEditPlayerViewControllerStatusObservationContext =
            new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");

        public PlaylistViewController() : base("PlaylistViewController", null)
        {
        }

        public PlaylistDTO Playlist { get; set; }
        public event ItemUpdateHandler<PlaylistDTO> ItemUpdated;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Delegate = this;
            TableView.DataSource = this;
            TableView.Editing = true;
            TableView.DragInteractionEnabled = true;
            TableView.AllowsSelectionDuringEditing = true;
            TableView.RegisterNibForCellReuse(SlimStoryCell.Nib, SlimStoryCell.Key);
            this.NavigationItem.Title = Playlist.Name;

            this.Preview.Layer.MasksToBounds = false;
            this.Preview.Layer.ShadowOffset = new CGSize(0, 2);
            this.Preview.Layer.ShadowRadius = 2;
            this.Preview.Layer.ShadowOpacity = 0.5f;

            this.ButtonsWrapper.Layer.MasksToBounds = false;
            this.ButtonsWrapper.Layer.ShadowOffset = new CGSize(0, 2);
            this.ButtonsWrapper.Layer.ShadowRadius = 2;
            this.ButtonsWrapper.Layer.ShadowOpacity = 0.5f;

            this.TogglePlayer(false);
        }

        private void TogglePlayer(bool show)
        {
            if (_playerVisible == show)
                return;


            this.NavigationItem.RightBarButtonItem = show
                ? new UIBarButtonItem(UIImage.FromBundle("Playlists"), UIBarButtonItemStyle.Done,
                    (x, y) => StopPlaying())
                : new UIBarButtonItem(UIBarButtonSystemItem.Play, PlayButton_Touched);
            this.NavigationItem.RightBarButtonItem.TintColor = UIColor.White;


            var height = show ? 64 : this.View.Frame.Height;
            UIView.Animate(0.2,
                () =>
                {
                    this.PlayerWrapperTop.Constant = height;
                    this.View.LayoutIfNeeded();
                }, () => { _playerVisible = show; });
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SlimStoryCell) tableView.DequeueReusableCell(SlimStoryCell.Key);
            cell.Story = Playlist.Stories[indexPath.Row];
            cell.ShowsReorderControl = true;
            return cell;
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return Playlist?.Stories.Count ?? 0;
        }

        [Export("tableView:canMoveRowAtIndexPath:")]
        // ReSharper disable once UnusedMember.Global, UnusedParameter.Global
        public bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        [Export("tableView:editingStyleForRowAtIndexPath:")]
        public UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }

        [Export("tableView:shouldIndentWhileEditingRowAtIndexPath:")]
        public bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SlimStoryCell) tableView.CellAt(indexPath);
            _currentItemIndex = Playlist.Stories.IndexOf(cell.Story);
            PlayStory(cell.Story);
        }

        [Export("tableView:moveRowAtIndexPath:toIndexPath:")]
        public void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var movedObject = this.Playlist.Stories[sourceIndexPath.Row];
            Playlist.Stories.RemoveAt(sourceIndexPath.Row);
            Playlist.Stories.Insert(destinationIndexPath.Row, movedObject);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == AvCustomEditPlayerViewControllerStatusObservationContext.Handle)
            {
                var playerItem = (AVPlayerItem) ofObject;
                if (playerItem.Status == AVPlayerItemStatus.ReadyToPlay)
                {
                    Spinner.StopAnimating();
                    Spinner.Hidden = true;
                    _player.Play();
                }
                else if (playerItem.Status == AVPlayerItemStatus.Failed)
                {
                    Spinner.StopAnimating();
                    Spinner.Hidden = true;
                    Console.WriteLine(playerItem.Error.LocalizedDescription);
                    StopPlaying();
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        partial void CloseButton_Activated(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }

        private void PlayButton_Touched(object sender, EventArgs eventArgs)
        {
            var currentItem = Playlist.Stories[_currentItemIndex];
            PlayStory(currentItem);
        }

        partial void NextButton_TouchUpInside(UIButton sender)
        {
            Next();
        }

        private void Next()
        {
            if (Playlist.Stories.Count - 1 > _currentItemIndex)
            {
                _currentItemIndex++;
                var story = Playlist.Stories[_currentItemIndex];
                PlayStory(story);
            }
            else
            {
                StopPlaying();
            }
        }

        partial void PreviousButton_TouchUpInside(UIButton sender)
        {
            if (_currentItemIndex > 0)
            {
                _currentItemIndex--;
                var story = Playlist.Stories[_currentItemIndex];
                PlayStory(story);
            }
        }

        private void PlayStory(StoryListDTO story)
        {
            this.TogglePlayer(true);
            Preview.SetImage(new NSUrl(story.PreviewUrl));

            if (_player == null)
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
                _player = new AVPlayer();
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = this.Preview.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                this.Preview.Layer.AddSublayer(_playerLayer);
            }

            var asset = this.GetAsset(story);
            _player.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(asset));

            _stopPlayingNotification =
                AVPlayerItem.Notifications.ObserveDidPlayToEndTime(_player.CurrentItem, DidReachEnd);
            _player.CurrentItem.AddObserver(this, "status",
                NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial,
                AvCustomEditPlayerViewControllerStatusObservationContext.Handle);
            Spinner.Hidden = false;
            Spinner.StartAnimating();
            _playing = true;
        }


        private AVUrlAsset GetAsset(StoryListDTO story)
        {
            NSUrl nSUrl = new NSUrl(story.VideoUrl);
            //var tempFileUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Append(Path.GetFileName(item.Url.ToString()), false);
            //tempFileUrl = NSUrl.CreateFileUrl(new[] { tempFileUrl.Path });

            var item = AVUrlAsset.Create(nSUrl);
            return item;
        }

        private void StopPlaying()
        {
            if (this._playing)
            {
                TogglePlayer(false);
                Spinner.StopAnimating();
                Spinner.Hidden = true;
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                _player.CurrentItem.RemoveObserver(this, "status",
                    AvCustomEditPlayerViewControllerStatusObservationContext.Handle);
                _playing = false;
                _player.Dispose();
                _playerLayer.Dispose();
                _player = null;
                _playerLayer = null;
            }
        }

        private void DidReachEnd(object sender, NSNotificationEventArgs e)
        {
            Next();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopPlaying();
            }

            base.Dispose(disposing);
        }
    }
}