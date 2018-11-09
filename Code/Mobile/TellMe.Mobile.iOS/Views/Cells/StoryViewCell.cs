using System;
using System.IO;
using AVFoundation;
using CoreFoundation;
using CoreMedia;
using Foundation;
using SDWebImage;
using TellMe.iOS.Core.UI;
using TellMe.Mobile.Core;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Types.Extensions;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class StoryViewCell : UITableViewCell, IUICollectionViewDataSource, IUICollectionViewDelegate, IAVAssetResourceLoaderDelegate
    {
        private AVUrlAsset _playerAsset;
        private AVPlayerItem _playerItem;
        private AVPlayer _player;
        private AVPlayerLayer _playerLayer;

        public static NSString AVCustomEditPlayerViewControllerStatusObservationContext =
            new NSString("AVCustomEditPlayerViewControllerStatusObservationContext");

        private NSObject _stopPlayingNotification;
        private bool _playing;
        private UIImage _defaultPicture;
        private StoryDTO _story;

        public static readonly NSString Key = new NSString("StoryViewCell");
        public static readonly UINib Nib;

        static StoryViewCell()
        {
            Nib = UINib.FromName("StoryViewCell", NSBundle.MainBundle);
        }

        public StoryDTO Story
        {
            get { return _story; }
            set
            {
                _story = value;
                this.Initialize();
            }
        }

        public Action<StoryDTO> OnPreviewTouched;
        public Action<StoryDTO> OnProfilePictureTouched;
        public Action<StoryDTO> OnLikeButtonTouched;
        public Action<StoryDTO> OnMoreButtonTouched;
        public Action<StoryReceiverDTO> OnReceiverSelected;

        public static StoryViewCell Create(StoryDTO story, bool hideMoreButton = false)
        {
            var arr = NSBundle.MainBundle.LoadNib("StoryViewCell", null, null);
            var v = ObjCRuntime.Runtime.GetNSObject<StoryViewCell>(arr.ValueAt(0));
            v.Story = story;
            v.MoreButton.Hidden = hideMoreButton;
            return v;
        }

        protected StoryViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this._defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
            this.Preview.UserInteractionEnabled = true;
            this.Preview.AddGestureRecognizer(new UITapGestureRecognizer(this.PreviewTouched));

            ReceiversCollection.DelaysContentTouches = false;
            ReceiversCollection.RegisterNibForCell(ReceiversListCell.Nib, ReceiversListCell.Key);
        }

        partial void MoreButton_Touched(UIButton sender)
        {
            this.OnMoreButtonTouched?.Invoke(Story);
        }

        public void Play()
        {
            if (!this._playing && !this.Story.Objectionable)
            {
                AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
                var cachedVideoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(Story.VideoUrl));
                _playerAsset = new AVUrlAsset(File.Exists(cachedVideoPath)
                    ? NSUrl.CreateFileUrl(new[] { cachedVideoPath })
                    : NSUrl.FromString(Story.VideoUrl));
                _playerAsset.ResourceLoader.SetDelegate(this, DispatchQueue.MainQueue);
                
                _playerItem = new AVPlayerItem(_playerAsset);
                _player = new AVPlayer(_playerItem);
                _playerLayer = AVPlayerLayer.FromPlayer(_player);
                _playerLayer.Frame = this.Preview.Bounds;
                _playerLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                this.Preview.Layer.AddSublayer(_playerLayer);
                _stopPlayingNotification =
                    AVPlayerItem.Notifications.ObserveDidPlayToEndTime(_player.CurrentItem, DidReachEnd);
                _player.CurrentItem.AddObserver(this, "status",
                    NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Initial,
                    AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                Spinner.Hidden = false;
                Spinner.StartAnimating();
                _playing = true;
            }
        }

        public void StopPlaying()
        {
            if (this._playing)
            {
                Spinner.StopAnimating();
                Spinner.Hidden = true;
                _stopPlayingNotification?.Dispose();
                _stopPlayingNotification = null;
                _player.Pause();
                _playerLayer.RemoveFromSuperLayer();
                _player.CurrentItem.RemoveObserver(this, "status",
                    AVCustomEditPlayerViewControllerStatusObservationContext.Handle);
                _playing = false;
            }
        }

        partial void ReplayButton_Touched(Button sender)
        {
            ReplayButton.Hidden = true;
            _player.Pause();
            _player.Seek(CMTime.Zero);
            _player.Play();
        }

        private void DidReachEnd(object sender, NSNotificationEventArgs e)
        {
            if (!_playerAsset.Url.IsFileUrl && _playerAsset.Exportable)
            {
                var exporter = new AVAssetExportSession(_playerAsset, AVAssetExportSessionPreset.Preset640x480);
                var videoPath = Path.Combine(Constants.TempVideoStorage, Path.GetFileName(Story.VideoUrl));
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

            ReplayButton.Hidden = false;
        }

        partial void LikeButton_TouchUpInside(Button sender)
        {
            this.OnLikeButtonTouched?.Invoke(Story);
        }

        public void RemoveTribe(TribeDTO tribe)
        {
            Story.Receivers.RemoveAll(x => x.TribeId == tribe.Id);
            InvokeOnMainThread(() => { ReceiversCollection.ReloadData(); });
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

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Story.Receivers?.Count ?? 0;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(ReceiversListCell.Key, indexPath) as ReceiversListCell;
            cell.Receiver = Story.Receivers[(int) indexPath.Item];
            cell.UserInteractionEnabled = true;
            return cell;
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath) as ReceiversListCell;
            if (cell != null)
            {
                this.OnReceiverSelected?.Invoke(cell.Receiver);
            }
        }

        private void ProfilePictureTouched()
        {
            this.OnProfilePictureTouched?.Invoke(Story);
        }

        private void PreviewTouched()
        {
            this.OnPreviewTouched?.Invoke(Story);
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Story.SenderPictureUrl, _defaultPicture);
            this.ObjectionableLabel.Hidden = !Story.Objectionable;
            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString($"{Story.SenderName} sent a story \""));

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Title.Font.PointSize),
                new NSRange(0, Story.SenderName.Length));
            text.Append(new NSAttributedString(Story.Title,
                font: UIFont.ItalicSystemFontOfSize(this.Title.Font.PointSize)));
            text.Append(new NSAttributedString("\" " + Story.CreateDateUtc.GetDateString(),
                foregroundColor: UIColor.LightGray));
            this.Title.AttributedText = text;
            this.Preview.SetImage(new NSUrl(Story.PreviewUrl));
            UpdateLikeButton(Story);

            ReceiversCollection.DataSource = this;
            ReceiversCollection.Delegate = this;
            ReceiversCollection.ReloadData();
        }

        public void UpdateLikeButton(StoryDTO targetStory)
        {
            this.Story.Liked = targetStory.Liked;
            this.Story.LikesCount = targetStory.LikesCount;
            this.LikeButton.SetTitle($"  {targetStory.LikesCount}", UIControlState.Normal);
            this.LikeButton.SetImage(UIImage.FromBundle(targetStory.Liked ? "Heart" : "Heart-O"),
                UIControlState.Normal);
            this.LikeButton.TintColor = targetStory.Liked ? UIColor.Red : UIColor.LightGray;
        }
        
        public void UpdateObjectionableState(bool objectionable)
        {
            this.ObjectionableLabel.Hidden = !objectionable;
            this.Story.Objectionable = objectionable;
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
                _defaultPicture.Dispose();
                _player = null;
                _playerItem = null;
                _playerLayer = null;
            }

            base.Dispose(disposing);
        }

        public bool ShouldWaitForLoadingOfRequestedResource(AVAssetResourceLoader resourceLoader,
            AVAssetResourceLoadingRequest loadingRequest)
        {
            return true;
        }
    }
}