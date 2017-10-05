using System;
using AVFoundation;
using Foundation;
using SDWebImage;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class StoriesListCell : UITableViewCell
    {
        AVPlayer _player;
        AVPlayerLayer _playerLayer;

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
        }

        StoryDTO story;
        private bool playing;

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

        private void PreviewTouched()
        {
            if (!this.playing)
            {
                _player = new AVPlayer(NSUrl.FromString(Story.VideoUrl));
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = Video.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                Video.Layer.AddSublayer(_playerLayer);
                Video.Hidden = false;
                _player.Play();
                playing = true;
            }

        }

        void VideoTouched()
        {
            StopPlaying();
        }

        private void StopPlaying()
        {
            if (this.playing)
            {
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
                this.Date.Text = Story.RequestDateUtc?.ToShortDateString();
                this.Title.Text = $"{story.ReceiverName} requested a story \"{Story.Title}\"";
            }
            else if (story.Status == StoryStatus.Sent)
            {
                this.Date.Text = Story.CreateDateUtc?.ToShortDateString();
                this.Title.Text = $"{story.SenderName} sent a story \"{Story.Title}\"";
            }
            else if (story.Status == StoryStatus.Ignored)
			{
                this.Date.Text = Story.UpdateDateUtc.ToShortDateString();
				this.Title.Text = $"{story.SenderName} ignored story request \"{Story.Title}\"";
			}

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


        public override void PrepareForReuse()
        {
            StopPlaying();
            _player?.Dispose();
            _playerLayer?.Dispose();
            _player = null;
            _playerLayer = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _player.Pause();
                _player.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
