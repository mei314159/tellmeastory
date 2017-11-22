using System;
using System.IO;
using AVFoundation;
using Foundation;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class PreviewVideoController : UIViewController, IView
    {
        private AVPlayer _player;
        private AVPlayerLayer _playerLayer;
        private AVAsset _asset;
        private AVPlayerItem _playerItem;

        private IRouter _router;
        private volatile bool _goNext;
        private NSObject _playerObserver;
        private string _previewImagePath;

        public PreviewVideoController(IntPtr handle) : base(handle)
        {
        }

        public string VideoPath { get; set; }
        public StoryRequestDTO StoryRequest { get; set; }
        public NotificationDTO RequestNotification { get; set; }
        public ContactDTO Contact { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _router = IoC.GetInstance<IRouter>();
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
            this._playerObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                AVPlayerItem.DidPlayToEndTimeNotification, (notify) =>
                {
                    _player.Seek(CoreMedia.CMTime.Zero);
                    notify.Dispose();
                });

            _player.Play();
        }

        partial void SendButtonTouched(Button sender)
        {
            _goNext = true;
            _router.NavigateStoryDetails(this, VideoPath, _previewImagePath, StoryRequest, RequestNotification,
                Contact);
        }

        public override void ViewWillDisappear(bool animated)
        {
            _player.Pause();
            NSNotificationCenter.DefaultCenter.RemoveObserver(_playerObserver);
            if (!_goNext)
            {
                this.DeleteFile(this.VideoPath);
                this.DeleteFile(this._previewImagePath);
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
                if (manager.FileExists(urlpath))
                {
                    Console.WriteLine("Deleting File");
                    manager.Remove(urlpath, out _);
                    Console.WriteLine("Deleted File");
                }
            }
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        private void SavePreviewImage()
        {
            var imageGenerator = AVAssetImageGenerator.FromAsset(_asset);

            imageGenerator.AppliesPreferredTrackTransform = true;

            var cmTime = new CoreMedia.CMTime(1, 30);

            var imageRef = imageGenerator.CopyCGImageAtTime(cmTime, out _, out _);
            if (imageRef == null)
                return;

            var image = UIImage.FromImage(imageRef);

            this._previewImagePath = Path.Combine(
                Path.GetDirectoryName(VideoPath).TrimEnd('/'),
                Path.GetFileNameWithoutExtension(VideoPath) + ".jpg");
            var dataBytes = image.AsJPEG().Save(this._previewImagePath, true, out _);
        }
    }
}