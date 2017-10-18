using System;
using System.IO;
using AVFoundation;
using CoreMedia;
using Foundation;
using SDWebImage;
using TellMe.Core;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
namespace TellMe.iOS.Views.Cells
{
    public partial class StoriesListCell : UITableViewCell
    {
        AVUrlAsset _playerAsset;
        AVPlayerItem _playerItem;
        AVPlayer _player;
        AVPlayerLayer _playerLayer;
        public static NSString AVCustomEditPlayerViewControllerStatusObservationContext = new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");
        public static readonly NSString Key = new NSString("StoriesListCell");
        public static readonly UINib Nib;

        static StoriesListCell()
        {
            Nib = UINib.FromName("StoriesListCell", NSBundle.MainBundle);
        }

        protected StoriesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            this.Preview.AddGestureRecognizer(new UITapGestureRecognizer(PreviewTouched));
            this.Video.AddGestureRecognizer(new UITapGestureRecognizer(VideoTouched));
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        StoryDTO story;
        private bool playing;
        private NSObject _stopPlayingNotification;

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

        private void PreviewTouched()
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
                _player.CurrentItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial,
                                            AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                Spinner.StartAnimating();
                Spinner.Hidden = false;
                playing = true;
            }

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
                    RestartPlayer();
                });
            }
            else
            {
                RestartPlayer();
            }
        }

        private void RestartPlayer()
        {
            _player.Pause();
            _player.Seek(CMTime.Zero);
            _player.Play();
        }

        void VideoTouched()
        {
            StopPlaying();
        }

        private void StopPlaying()
        {
            if (this.playing)
            {
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                Video.Hidden = true;
                playing = false;
            }
        }

        private void Initialize()
        {
            if (story.Status == StoryStatus.Requested)
            {
                this.Date.Text = Story.RequestDateUtc?.GetDateString();
                this.Title.Text = $"{story.ReceiverName} requested a story \"{Story.Title}\"";
                this.ProfilePicture.SetPictureUrl(story.ReceiverPictureUrl, defaultPicture);
            }
            else if (story.Status == StoryStatus.Sent)
            {
                this.Date.Text = Story.CreateDateUtc?.GetDateString();
                this.Title.Text = $"{story.SenderName} sent a story \"{Story.Title}\"";
                this.ProfilePicture.SetPictureUrl(story.SenderPictureUrl, defaultPicture);
            }
            else if (story.Status == StoryStatus.Ignored)
            {
                this.Date.Text = Story.UpdateDateUtc.GetDateString();
                this.Title.Text = $"{story.SenderName} ignored story request \"{Story.Title}\"";
                this.ProfilePicture.SetPictureUrl(story.SenderPictureUrl, defaultPicture);
            }
            else
            {
                this.ProfilePicture.SetPictureUrl(null, defaultPicture);
            }

            Spinner.Hidden = true;
            Preview.Hidden = story.Status != StoryStatus.Sent;
            if (story.Status == StoryStatus.Sent)
            {
                this.Preview.SetImage(new NSUrl(story.PreviewUrl));
            }
            else
            {
                this.Preview.Image = null;
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


        public override void PrepareForReuse()
        {
            StopPlaying();
            _player?.Dispose();
            _playerItem?.Dispose();
            _playerLayer?.Dispose();
            _player = null;
            _playerItem = null;
            _playerLayer = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
