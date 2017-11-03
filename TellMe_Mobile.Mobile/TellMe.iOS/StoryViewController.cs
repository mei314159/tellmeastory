using System;
using System.IO;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using SDWebImage;
using TellMe.Core;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class StoryViewController : UIViewController, IView
    {
        private UIVisualEffectView effectView;
        private UIVisualEffectView EffectView => effectView ?? (effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Light)));
        AVUrlAsset _playerAsset;
        AVPlayerItem _playerItem;
        AVPlayer _player;
        AVPlayerLayer _playerLayer;
        private NSObject _stopPlayingNotification;
        private bool playing;

        public static NSString AVCustomEditPlayerViewControllerStatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");


        public StoryViewController(IntPtr handle) : base(handle)
        {
        }

        private CGRect originalImageFrame;

        public StoryDTO Story { get; set; }

        public override void ViewDidLoad()
        {
            Initialize();
            EffectView.Frame = View.Bounds;
            View.AddSubview(EffectView);
            View.SendSubviewToBack(EffectView);
            var swipeGestureRecognizer = new UIPanGestureRecognizer(HandleAction);
            swipeGestureRecognizer.ShouldRecognizeSimultaneously += (gestureRecognizer, otherGestureRecognizer) => true;
            this.View.AddGestureRecognizer(swipeGestureRecognizer);
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Story.SenderPictureUrl, UIImage.FromBundle("UserPic"));
            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString($"{Story.SenderName} sent a story \""));

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Title.Font.PointSize), new NSRange(0, Story.SenderName.Length));
            text.Append(new NSAttributedString(Story.Title, font: UIFont.ItalicSystemFontOfSize(this.Title.Font.PointSize)));
            text.Append(new NSAttributedString("\" " + Story.CreateDateUtc.GetDateString(), foregroundColor: UIColor.LightGray));
            this.Title.AttributedText = text;
            this.Preview.SetImage(new NSUrl(Story.PreviewUrl));
            //this.ProfilePicture.UserInteractionEnabled = true;
            //this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(() => App.Instance.Router.NavigateStoryteller(this, Story.SenderId)));
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override void ViewDidAppear(bool animated)
        {
            Play(this.StoryViewWrapper);
        }

        public override void ViewWillDisappear(bool animated)
        {
            StopPlaying();
        }

        void HandleAction(UIPanGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                originalImageFrame = View.Frame;
            }

            // Move the image if the gesture is valid
            if (recognizer.State != UIGestureRecognizerState.Cancelled && recognizer.State != UIGestureRecognizerState.Failed && recognizer.State != UIGestureRecognizerState.Possible)
            {
                // Move the image by adding the offset to the object's frame
                var offset = recognizer.TranslationInView(View);
                if (offset.Y < 0)
                    return;

                var newFrame = originalImageFrame;
                newFrame.Offset(0, offset.Y);
                View.Frame = newFrame;
                if (offset.Y > 150 || recognizer.State == UIGestureRecognizerState.Ended)
                {
                    this.DismissViewController(true, null);
                }
            }
        }

        public void Play(UIView target)
        {
            if (!this.playing)
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
                var cachedVideoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(Story.VideoUrl));
                _playerAsset = new AVUrlAsset(File.Exists(cachedVideoPath) ? new NSUrl(cachedVideoPath, false) : NSUrl.FromString(Story.VideoUrl));
                _playerItem = new AVPlayerItem(_playerAsset);
                _player = new AVPlayer(_playerItem);
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = target.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                target.Layer.AddSublayer(_playerLayer);
                _stopPlayingNotification = AVPlayerItem.Notifications.ObserveDidPlayToEndTime(_player.CurrentItem, DidReachEnd);
                _player.CurrentItem.AddObserver(this, "status", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial,
                                            AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                Spinner.Hidden = false;
                Spinner.StartAnimating();
                playing = true;
            }
        }

        public void StopPlaying()
        {
            if (this.playing)
            {
                Spinner.StopAnimating();
                Spinner.Hidden = true;
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                _player.CurrentItem.RemoveObserver(this, "status", AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                playing = false;
            }
        }


        public void Restart()
        {
            _player.Pause();
            _player.Seek(CMTime.Zero);
            _player.Play();
        }

        void DidReachEnd(object sender, NSNotificationEventArgs e)
        {
            if (!_playerAsset.Url.IsFileUrl && _playerAsset.Exportable)
            {
                var exporter = new AVAssetExportSession(_playerAsset, AVAssetExportSessionPreset.Preset640x480);
                var videoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(Story.VideoUrl));
                if (File.Exists(videoPath))
                {
                    return;
                }
                var startTime = new CMTime(0, 1);
                var timeRange = new CMTimeRange();
                timeRange.Start = startTime;
                timeRange.Duration = _playerItem.Duration;
                exporter.TimeRange = timeRange;
                exporter.OutputUrl = new NSUrl(videoPath, false);
                exporter.OutputFileType = AVFileType.QuickTimeMovie;
                exporter.ExportAsynchronously(() =>
                {
                    Console.WriteLine(exporter.Status);
                    Console.WriteLine(exporter.Error);

                    Console.WriteLine(exporter.Description);
                    Console.WriteLine(exporter.DebugDescription);

                    Restart();
                });
            }
            else
            {
                Restart();
            }
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            var ch = new NSObservedChange(change);
            if (context == AVCustomEditPlayerViewControllerStatusObservationContext.Handle)
            {
                AVPlayerItem playerItem = ofObject as AVPlayerItem;
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

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);
        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopPlaying();
                _player.Pause();
                _player.Dispose();
                _playerItem?.Dispose();
                _playerLayer?.Dispose();
                _player = null;
                _playerItem = null;
                _playerLayer = null;
            }

            base.Dispose(disposing);
        }
    }
}
