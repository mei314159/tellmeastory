using System;

using Foundation;
using Newtonsoft.Json.Linq;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
using SDWebImage;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.iOS.Views.Cells
{
    public partial class NotificationCenterStoryCell : UITableViewCell, INotificationCenterCell
    {
        public static readonly NSString Key = new NSString("NotificationCenterStoryCell");
        public static readonly UINib Nib;

        static NotificationCenterStoryCell()
        {
            Nib = UINib.FromName("NotificationCenterStoryCell", NSBundle.MainBundle);
        }

        protected NotificationCenterStoryCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        private NotificationDTO notification;

        public NotificationDTO Notification
        {
            get
            {
                return notification;
            }

            set
            {
                notification = value;
                Initialize();
            }
        }


        private void Initialize()
        {
            if (Notification.Type != NotificationTypeEnum.Story)
            {
                return;
            }

            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString(Notification.Text));
            nint index = default(nint);
            nint length = default(nint);

            var storyDTO = ((JObject)notification.Extra).ToObject<StoryDTO>();
            index = Notification.Text.IndexOf(storyDTO.SenderName, StringComparison.Ordinal);
            length = storyDTO.SenderName.Length;
            PictureView.SetImage(new NSUrl(storyDTO.SenderPictureUrl));
            StoryPreview.SetImage(new NSUrl(storyDTO.PreviewUrl));
            if (index >= 0)
            {
                text.AddAttribute(UIStringAttributeKey.Font,
                                  UIFont.BoldSystemFontOfSize(this.Text.Font.PointSize),
                                  new NSRange(index, length));
            }

            text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(), foregroundColor: UIColor.LightGray));
            this.Text.AttributedText = text;
        }
    }
}
