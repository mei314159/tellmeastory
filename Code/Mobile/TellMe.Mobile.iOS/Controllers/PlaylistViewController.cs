using System;
using System.IO;
using System.Threading;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using SDWebImage;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Shared.Contracts.DTO;
using UIKit;
using Constants = TellMe.Mobile.Core.Constants;
using Timer = System.Timers.Timer;

namespace TellMe.iOS.Controllers
{
    public partial class PlaylistViewController : UIViewController, IPlaylistView, IUITableViewDelegate,
        IUITableViewDataSource,
        IAVAssetResourceLoaderDelegate
    {
        private AVPlayer _player;
        private AVPlayerLayer _playerLayer;
        private NSObject _stopPlayingNotification;
        private bool _playing;
        private int _currentItemIndex;
        private bool _playerVisible = true;

        private static readonly NSString AvCustomEditPlayerViewControllerStatusObservationContext =
            new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");

        private UIColor _navbarWrapperColor;
        private bool _controlsVisible = true;
        private UIBarButtonItem _saveButton;
        private UIBarButtonItem _editButton;
        private UIBarButtonItem _shareButton;
        private UIBarButtonItem _closeButton;
        private Timer _timer;
        private IPlaylistViewBusinessLogic _businessLogic;

        public PlaylistViewController() : base("PlaylistViewController", null)
        {
        }

        public PlaylistDTO Playlist { get; set; }
        public event ItemUpdateHandler<PlaylistDTO> ItemUpdated;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = IoC.GetInstance<IPlaylistViewBusinessLogic>();
            this._businessLogic.View = this;
            TableView.Delegate = this;
            TableView.DataSource = this;
            TableView.DragInteractionEnabled = true;
            TableView.AllowsSelectionDuringEditing = true;
            TableView.RegisterNibForCellReuse(SlimStoryCell.Nib, SlimStoryCell.Key);
            SetGestureRecognizers();
            this._navbarWrapperColor = UIColor.FromRGB(33, 194, 250).ColorWithAlpha(0.4f);
            this.ButtonsWrapper.BackgroundColor = _navbarWrapperColor;
            this.NavigationController.NavigationBar.BarTintColor = _navbarWrapperColor;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
            this._timer = new Timer(3000);
            this._timer.Elapsed += (sender, args) =>
            {
                if (_playing)
                    InvokeOnMainThread(() =>
                        ToggleControls(false));
            };
            this.SetButtons();
            this.TogglePlayer(false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            StopPlaying();
        }

        private void TogglePlayer(bool show)
        {
            if (_playerVisible == show)
                return;

            if (show)
            {
                ToggleControls(true);
            }
            else
            {
                this.NavigationController.SetNavigationBarHidden(false, true);
            }
            var top = show ? 0 : this.View.Frame.Height;
            UIView.Animate(0.2,
                () =>
                {
                    this.PlayerWrapperTop.Constant = top;
                    this.View.LayoutIfNeeded();
                }, () =>
                {
                    {
                        _playerVisible = show;
                    }
                });
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

        public bool ShouldWaitForLoadingOfRequestedResource(AVAssetResourceLoader resourceLoader,
            AVAssetResourceLoadingRequest loadingRequest)
        {
            return true;
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public void PlaylistSaved()
        {
            TableView.SetEditing(false, true);
            SetButtons();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopPlaying();
                this._saveButton.Dispose();
                this._editButton.Dispose();
                this._shareButton.Dispose();
                _navbarWrapperColor.Dispose();
                this._saveButton = null;
                this._editButton = null;
                this._shareButton = null;
                _navbarWrapperColor = null;
            }

            base.Dispose(disposing);
        }

        partial void NextButton_TouchUpInside(UIButton sender)
        {
            Next();
        }

        partial void PreviousButton_TouchUpInside(UIButton sender)
        {
            Previous();
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

        private void Previous()
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
            _playing = true;
            this.TogglePlayer(true);
            SetButtons();
            Preview.SetImage(new NSUrl(story.PreviewUrl));

            if (_player == null)
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
                _player = new AVPlayer();
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = this.Preview.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
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
        }


        private AVUrlAsset GetAsset(StoryListDTO story)
        {
            NSUrl videoUrl;

            var videoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(story.VideoUrl));
            if (File.Exists(videoPath))
            {
                videoUrl = NSUrl.CreateFileUrl(new[] {videoPath});
            }
            else
            {
                videoUrl = new NSUrl(story.VideoUrl);
            }

            var item = AVUrlAsset.Create(videoUrl);
            item.ResourceLoader.SetDelegate(this, DispatchQueue.MainQueue);
            return item;
        }

        private void StopPlaying()
        {
            if (this._playing)
            {
                _playing = false;
                SetButtons();
                TogglePlayer(false);
                Spinner.StopAnimating();
                Spinner.Hidden = true;
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                _player.CurrentItem.RemoveObserver(this, "status",
                    AvCustomEditPlayerViewControllerStatusObservationContext.Handle);
                _player.Dispose();
                _playerLayer.Dispose();
                _player = null;
                _playerLayer = null;
            }
        }

        private void DidReachEnd(object sender, NSNotificationEventArgs e)
        {
            var asset = (AVUrlAsset) _player.CurrentItem.Asset;
            if (!asset.Url.IsFileUrl && asset.Exportable)
            {
                var exporter = new AVAssetExportSession(asset, AVAssetExportSessionPreset.Preset640x480);
                var videoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(asset.Url.ToString()));
                if (File.Exists(videoPath))
                {
                    return;
                }

                exporter.OutputUrl = NSUrl.CreateFileUrl(new[] {videoPath});
                exporter.OutputFileType = AVFileType.QuickTimeMovie;
                exporter.ExportAsynchronously(() =>
                {
                    Console.WriteLine(exporter.Status);
                    Console.WriteLine(exporter.Error);

                    Console.WriteLine(exporter.Description);
                    Console.WriteLine(exporter.DebugDescription);
                });
            }
            Next();
        }

        private void SetGestureRecognizers()
        {
            this.Preview.AddGestureRecognizer(new UITapGestureRecognizer(this.PreviewTouched)
            {
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true
            });
            this.Preview.AddGestureRecognizer(new UISwipeGestureRecognizer(this.PreviewSwiped)
            {
                Direction = UISwipeGestureRecognizerDirection.Left,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true
            });
            this.Preview.AddGestureRecognizer(new UISwipeGestureRecognizer(this.PreviewSwiped)
            {
                Direction = UISwipeGestureRecognizerDirection.Right,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true
            });
            this.Preview.AddGestureRecognizer(new UISwipeGestureRecognizer(this.PreviewSwiped)
            {
                Direction = UISwipeGestureRecognizerDirection.Up,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true
            });
            this.Preview.AddGestureRecognizer(new UISwipeGestureRecognizer(this.PreviewSwiped)
            {
                Direction = UISwipeGestureRecognizerDirection.Down,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true
            });
        }

        private void SetButtons()
        {
            if (_closeButton == null)
                _closeButton = new UIBarButtonItem(UIBarButtonSystemItem.Stop, CloseButton_Touched);
            if (_saveButton == null)
                _saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveButton_Touched);
            if (_editButton == null)
                _editButton = new UIBarButtonItem(UIBarButtonSystemItem.Edit, EditButton_Touched);
            if (_shareButton == null)
                _shareButton = new UIBarButtonItem(UIBarButtonSystemItem.Action, ShareButton_Touched);

            if (_playing)
            {
                this.NavigationController.NavigationBar.TopItem.RightBarButtonItems = null;
                this.NavigationController.NavigationBar.TopItem.Title = null;
                this.NavigationController.NavigationBar.ShadowImage = new UIImage();
                this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
                return;
            }
            this.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
            this.NavigationController.NavigationBar.TopItem.LeftBarButtonItem = _closeButton;
            this.NavigationController.NavigationBar.TopItem.Title = Playlist.Name;

            if (_businessLogic.CanSaveOrder)
            {
                if (TableView.Editing)
                {
                    this.NavigationController.NavigationBar.TopItem.RightBarButtonItems = new[]
                    {
                        _saveButton, _shareButton
                    };
                }
                else
                {
                    this.NavigationController.NavigationBar.TopItem.RightBarButtonItems = new[]
                    {
                        _editButton, _shareButton
                    };
                }
            }
            else
            {
                this.NavigationController.NavigationBar.TopItem.RightBarButtonItem = _shareButton;
            }
        }

        private void EditButton_Touched(object sender, EventArgs eventArgs)
        {
            TableView.SetEditing(true, true);
            SetButtons();
        }

        private async void SaveButton_Touched(object sender, EventArgs eventArgs)
        {
            await _businessLogic.SaveAsync().ConfigureAwait(false);
        }

        private void ShareButton_Touched(object sender, EventArgs e)
        {
            _businessLogic.Share();
        }

        private void CloseButton_Touched(object sender, EventArgs eventArgs)
        {
            if (_playing)
            {
                StopPlaying();
            }
            else
            {
                this.DismissViewController(true, null);
            }
        }

        private void PreviewTouched(UITapGestureRecognizer r)
        {
            ToggleControls();
        }

        private void PreviewSwiped(UISwipeGestureRecognizer r)
        {
            switch (r.Direction)
            {
                case UISwipeGestureRecognizerDirection.Right:
                    this.Previous();
                    break;
                case UISwipeGestureRecognizerDirection.Left:
                    this.Next();
                    break;
                case UISwipeGestureRecognizerDirection.Up:
                    ToggleControls(true);
                    break;
                case UISwipeGestureRecognizerDirection.Down:
                    StopPlaying();
                    break;
            }
        }

        private void ToggleControls(bool? visible = null)
        {
            var newValue = visible ?? !_controlsVisible;

            var alpha = newValue ? 1 : 0;

            this.NavigationController.SetNavigationBarHidden(_playing && !newValue, true);
            UIView.Animate(0.2, () =>
            {
                this.ButtonsWrapper.Alpha = alpha;
                this.View.LayoutIfNeeded();
            }, () =>
            {
                _controlsVisible = newValue;
                if (newValue)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            });
        }
    }
}