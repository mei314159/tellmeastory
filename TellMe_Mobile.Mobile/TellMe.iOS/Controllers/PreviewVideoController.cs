using Foundation;
using System;
using UIKit;
using AVFoundation;
using TellMe.Core.Contracts;
using TellMe.Core;
using TellMe.Core.Contracts.UI.Views;
using System.IO;

namespace TellMe.iOS
{
    public partial class PreviewVideoController : UIViewController, IView
    {
        AVPlayer _player;
        AVPlayerLayer _playerLayer;
        AVAsset _asset;
        AVPlayerItem _playerItem;

        IRouter _router;
        private volatile bool goNext;
        private NSObject playerObserver;
        private string previewImagePath;

        public PreviewVideoController(IntPtr handle) : base(handle)
        {
        }

        public string VideoPath { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _router = App.Instance.Router;

            var url = new NSUrl(VideoPath, false);

            _asset = AVAsset.FromUrl(url);
            _playerItem = new AVPlayerItem(_asset);
            _player = new AVPlayer(_playerItem);
            _playerLayer = AVPlayerLayer.FromPlayer(_player);
            _playerLayer.Frame = PreviewView.Bounds;
            PreviewView.Layer.AddSublayer(_playerLayer);
            SavePreviewImage();
        }

        public override void ViewDidAppear(bool animated)
        {
			this.playerObserver = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, (notify) =>
			{
				_player.Seek(CoreMedia.CMTime.Zero);
				notify.Dispose();
			});

			_player.Play();
        }

        partial void SendButtonTouched(Button sender)
        {
            goNext = true;
            _router.NavigateStoryDetails(this, VideoPath, previewImagePath);
        }

        public override void ViewWillDisappear(bool animated)
        {
            _player.Pause();
            NSNotificationCenter.DefaultCenter.RemoveObserver(playerObserver);
            if (!goNext)
            {
                this.DeleteFile(this.VideoPath);
                this.DeleteFile(this.previewImagePath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _player.Pause();
                _asset.Dispose();
                _playerItem.Dispose();
                _player.Dispose();
            }

            base.Dispose(disposing);
        }

        private void DeleteFile(string urlpath)
        {
            using (var manager = new NSFileManager())
            {
                NSError error = new NSError();
                if (manager.FileExists(urlpath))
                {
                    Console.WriteLine("Deleting File");
                    manager.Remove(urlpath, out error);
                    Console.WriteLine("Deleted File");
                }
            }
        }

		public void ShowErrorMessage(string title, string message = null)
		{
			InvokeOnMainThread(() =>
			{
				UIAlertController alert = UIAlertController
					.Create(title,
							message ?? string.Empty,
							UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
				this.PresentViewController(alert, true, null);
			});
		}

        public void SavePreviewImage()
		{
			var imageGenerator = AVAssetImageGenerator.FromAsset(_asset);

			imageGenerator.AppliesPreferredTrackTransform = true;

            var actualTime = _asset.Duration;
			var cmTime = new CoreMedia.CMTime(1, 30);

            var imageRef = imageGenerator.CopyCGImageAtTime(cmTime, out actualTime, out NSError error);
            if (imageRef == null)
				return;

			var image = UIImage.FromImage(imageRef);

            this.previewImagePath = Path.Combine(
                Path.GetDirectoryName(VideoPath).TrimEnd('/'), 
                Path.GetFileNameWithoutExtension(VideoPath) + ".jpg");
            var dataBytes = image.AsJPEG().Save(this.previewImagePath, true, out error);
		}
    }
}