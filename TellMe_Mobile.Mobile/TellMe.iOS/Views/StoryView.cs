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

namespace TellMe.iOS
{
    public partial class StoryView : UIView
    {
        AVUrlAsset _playerAsset;
        AVPlayerItem _playerItem;
        AVPlayer _player;
        AVPlayerLayer _playerLayer;
        private NSObject _stopPlayingNotification;
        private StoryDTO story;
        private bool playing;
        private UIVisualEffectView effectView;
        private UIVisualEffectView EffectView => effectView ?? (effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark)));

        public static NSString AVCustomEditPlayerViewControllerStatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
        public static readonly NSString Key = new NSString("StoryView");
        public static readonly UINib Nib;


        public StoryView(IntPtr handle) : base(handle)
        {
            NSBundle.MainBundle.LoadNib(Key, this, null);
            this.AddSubview(this.View);
            //this.Preview.AddGestureRecognizer(new UITapGestureRecognizer(PreviewTouched));
            //this.Video.AddGestureRecognizer(new UITapGestureRecognizer(VideoTouched));
            //this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        public StoryView(NSCoder coder) : base(coder)
        {
            NSBundle.MainBundle.LoadNib(Key, this, null);
            this.AddSubview(this.View);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(StoryTouched));
            var swipeGestureRecognizer = new UISwipeGestureRecognizer(SwipedDown);
            swipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Down | UISwipeGestureRecognizerDirection.Up;
            swipeGestureRecognizer.NumberOfTouchesRequired = 1;
            swipeGestureRecognizer.ShouldRecognizeSimultaneously += (gestureRecognizer, otherGestureRecognizer) => true;
            this.View.AddGestureRecognizer(swipeGestureRecognizer);
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        public StoryDTO Story
        {
            get
            {
                return story;
            }
            set
            {
                story = value;
                this.Initialize();
            }
        }

        private UIImage defaultPicture;
        private bool expanded;
        private UIView cellSuperview;
        private CGRect initialFrame;
        private CGSize initialSize;

        private void StoryTouched()
        {
            if (!this.expanded)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }

        void SwipedDown()
        {
            if (expanded)
            {
                Collapse();
            }
        }

        private void Collapse()
        {
            StopPlaying();
            Animate(0.15, () =>
            {
                View.Frame = initialFrame;
                ViewHeight.Constant = initialFrame.Height;
                View.Superview.LayoutIfNeeded();
                effectView.Alpha = 0;
            }, () =>
            {
                View.RemoveFromSuperview();
                View.Frame = new CGRect(CGPoint.Empty, initialSize);
                this.AddSubview(View);
                effectView.RemoveFromSuperview();
                expanded = false;
            });
        }

        private void Expand()
        {
            var window = Window;
            this.initialSize = View.Frame.Size;
            this.initialFrame = new CGRect(window.ConvertPointFromView(Frame.Location, View), initialSize);

            EffectView.Alpha = 0;
            EffectView.Frame = window.Bounds;
            window.EndEditing(true);
            window.AddSubview(EffectView);
            this.cellSuperview = View.Superview;
            View.RemoveFromSuperview();
            window.AddSubview(View);
            View.Frame = initialFrame;
            nfloat delta = (View.Frame.Size.Width) / 3;

            Animate(0.25, () =>
            {
                //Transform = CGAffineTransform.MakeScale(1, 1);
                EffectView.Alpha = 0.8f;
                View.Frame = new CGRect(new CGPoint(0, 20), new CGSize(View.Frame.Size.Width, View.Frame.Size.Height + delta));
                ViewHeight.Constant = View.Frame.Height;
                View.Superview.LayoutIfNeeded();
            }, () =>
            {
                expanded = true;
                Play();
            });
        }

        public void Play()
        {
            if (!this.playing)
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
                var cachedVideoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(story.VideoUrl));
                _playerAsset = new AVUrlAsset(File.Exists(cachedVideoPath) ? new NSUrl(cachedVideoPath, false) : NSUrl.FromString(Story.VideoUrl));
                _playerItem = new AVPlayerItem(_playerAsset);
                _player = new AVPlayer(_playerItem);
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = Video.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                Video.Layer.AddSublayer(_playerLayer);
                _stopPlayingNotification = AVPlayerItem.Notifications.ObserveDidPlayToEndTime(_player.CurrentItem, DidReachEnd);
                _player.CurrentItem.AddObserver(this, "status", NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial,
                                            AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                Spinner.StartAnimating();
                Spinner.Hidden = false;
                playing = true;
            }
        }

        public void StopPlaying()
        {
            if (this.playing)
            {
                Spinner.Hidden = true;
                Spinner.StopAnimating();
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                _player.CurrentItem.RemoveObserver(this, "status", AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                Video.Hidden = true;
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
            var asset = (AVUrlAsset)_playerItem.Asset;
            if (!asset.Url.IsFileUrl)
            {
                var exporter = new AVAssetExportSession(_player.CurrentItem.Asset, AVAssetExportSessionPreset.HighestQuality);
                var videoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(story.VideoUrl));
                exporter.OutputUrl = new NSUrl(videoPath, false);
                exporter.OutputFileType = AVFileType.QuickTimeMovie;
                exporter.ExportAsynchronously(() =>
                {
                    Console.WriteLine(exporter.Status);
                    Console.WriteLine(exporter.Error);
                    Restart();
                });
            }
            else
            {
                Restart();
            }
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(story.SenderPictureUrl, defaultPicture);
            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString($"{story.SenderName} sent a story \""));

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Title.Font.PointSize), new NSRange(0, story.SenderName.Length));
            text.Append(new NSAttributedString(Story.Title, font: UIFont.ItalicSystemFontOfSize(this.Title.Font.PointSize)));
            text.Append(new NSAttributedString("\" " + Story.CreateDateUtc.GetDateString(), foregroundColor: UIColor.LightGray));
            this.Title.AttributedText = text;
            Spinner.Hidden = true;
            Preview.Hidden = false;
            this.Preview.SetImage(new NSUrl(story.PreviewUrl));
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            var ch = new NSObservedChange(change);
            if (context == AVCustomEditPlayerViewControllerStatusObservationContext.Handle)
            {
                AVPlayerItem playerItem = ofObject as AVPlayerItem;
                if (playerItem.Status == AVPlayerItemStatus.ReadyToPlay)
                {
                    Video.Hidden = false;
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