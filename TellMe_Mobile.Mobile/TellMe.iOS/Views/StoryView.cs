using System;
using Foundation;
using SDWebImage;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;

namespace TellMe.iOS
{
    public partial class StoryView : UIView
    {
        private UIImage defaultPicture;
        private StoryDTO story;
        public static readonly NSString Key = new NSString("StoryView");
        public static readonly UINib Nib;

        public StoryView(IntPtr handle) : base(handle)
        {
            NSBundle.MainBundle.LoadNib(Key, this, null);
            this.AddSubview(this.View);
        }

        public StoryView(NSCoder coder) : base(coder)
        {
            NSBundle.MainBundle.LoadNib(Key, this, null);
            this.AddSubview(this.View);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
            this.Preview.UserInteractionEnabled = true;
            this.Preview.AddGestureRecognizer(new UITapGestureRecognizer(this.PreviewTouched));
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

        public event Action<StoryDTO> OnPreviewTouched;

        public event Action<StoryDTO> OnProfilePictureTouched;

        public static StoryView Create(StoryDTO story)
        {
            var arr = NSBundle.MainBundle.LoadNib("StoryView", null, null);
            var v = ObjCRuntime.Runtime.GetNSObject<StoryView>(arr.ValueAt(0));
            v.Story = story;
            return v;
        }

        void ProfilePictureTouched()
        {
            this.OnProfilePictureTouched?.Invoke(Story);
        }

        void PreviewTouched()
        {
            this.OnPreviewTouched?.Invoke(Story);
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Story.SenderPictureUrl, defaultPicture);
            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString($"{Story.SenderName} sent a story \""));

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Title.Font.PointSize), new NSRange(0, Story.SenderName.Length));
            text.Append(new NSAttributedString(Story.Title, font: UIFont.ItalicSystemFontOfSize(this.Title.Font.PointSize)));
            text.Append(new NSAttributedString("\" " + Story.CreateDateUtc.GetDateString(), foregroundColor: UIColor.LightGray));
            this.Title.AttributedText = text;
            this.Preview.SetImage(new NSUrl(Story.PreviewUrl));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                defaultPicture.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}