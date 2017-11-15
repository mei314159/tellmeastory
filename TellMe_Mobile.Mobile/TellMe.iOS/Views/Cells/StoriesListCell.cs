using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
using SDWebImage;

namespace TellMe.iOS.Views.Cells
{
    public partial class StoriesListCell : UITableViewCell, IUICollectionViewDataSource, IUICollectionViewDelegate
    {
        private UIImage defaultPicture;
        private StoryDTO story;

        public static readonly NSString Key = new NSString("StoriesListCell");
        public static readonly UINib Nib;

        public Action<StoryDTO> ProfilePictureTouched { get; set; }
        public Action<StoryDTO> PreviewTouched { get; set; }
        public Action<StoryDTO> CommentsButtonTouched { get; set; }
        public Action<StoryDTO> LikeButtonTouched { get; set; }
        public Action<StoryReceiverDTO, StoriesListCell> ReceiverSelected { get; set; }

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
            base.AwakeFromNib();

            this.defaultPicture = UIImage.FromBundle("UserPic");

            this.AddGestureRecognizer(new UITapGestureRecognizer(UIViewTouched)
            {
                CancelsTouchesInView = false
            });

            ReceiversCollection.DelaysContentTouches = false;
            ReceiversCollection.RegisterNibForCell(ReceiversListCell.Nib, ReceiversListCell.Key);
        }

        public StoryDTO Story
        {
            get { return story; }
            set
            {
                story = value;
                this.Initialize();
            }
        }

        private void UIViewTouched(UITapGestureRecognizer r)
        {
            var location = r.LocationOfTouch(0, this);
            if (ProfilePicture.Frame.Contains(location))
            {
                this.ProfilePictureTouched?.Invoke(Story);
            }
            else if (Preview.Frame.Contains(location))
            {
                this.PreviewTouched?.Invoke(Story);
            }
        }

        partial void LikeButton_TouchUpInside(Button sender)
        {
            this.LikeButtonTouched?.Invoke(Story);
        }

        partial void CommentsButton_TouchUpInside(Button sender)
        {
            this.CommentsButtonTouched?.Invoke(Story);
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Story.SenderPictureUrl, defaultPicture);
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
            this.CommentsButton.SetTitle($"  {Story.CommentsCount}", UIControlState.Normal);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                defaultPicture.Dispose();
            }

            base.Dispose(disposing);
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Story.Receivers?.Count ?? 0;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(ReceiversListCell.Key, indexPath) as ReceiversListCell;
            cell.Receiver = Story.Receivers[(int) indexPath.Item];
            return cell;
        }

        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath) as ReceiversListCell;
            if (cell != null)
            {
                this.ReceiverSelected?.Invoke(cell.Receiver, this);
            }
        }

        public void RemoveTribe(TribeDTO tribe)
        {
            Story.Receivers.RemoveAll(x => x.TribeId == tribe.Id);
            InvokeOnMainThread(() => { ReceiversCollection.ReloadData(); });
        }
    }
}