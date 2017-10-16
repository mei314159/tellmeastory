using System;
using CoreText;
using Foundation;
using Newtonsoft.Json.Linq;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
using SDWebImage;

namespace TellMe.iOS.Views.Cells
{
    public partial class NotificationCenterCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("NotificationCenterCell");
        public static readonly UINib Nib;

        static NotificationCenterCell()
        {
            Nib = UINib.FromName("NotificationCenterCell", NSBundle.MainBundle);
        }

        protected NotificationCenterCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        NotificationDTO notification;

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
            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString(Notification.Text));
            nint index = default(nint);
            nint length = default(nint);
            switch (Notification.Type)
            {
                case NotificationTypeEnum.StoryRequest:
                    var storyDTO = ((JObject)notification.Extra).ToObject<StoryDTO>();
                    index = Notification.Text.IndexOf(storyDTO.ReceiverName, StringComparison.Ordinal);
                    length = storyDTO.ReceiverName.Length;
                    PictureView.SetImage(new NSUrl(storyDTO.ReceiverPictureUrl));
                    break;
                case NotificationTypeEnum.FriendshipRequest:
                    var dto = ((JObject)notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    if (!Notification.Handled)
                    {
                        this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }

                    PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                case NotificationTypeEnum.FriendshipAccepted:
                    dto = ((JObject)notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                case NotificationTypeEnum.FriendshipRejected:
                    dto = ((JObject)notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                default:
                    this.Text.AttributedText = text;
                    text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(), foregroundColor: UIColor.LightGray));
                    return;
            }

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Text.Font.PointSize), new NSRange(index, length));
            text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(), foregroundColor: UIColor.LightGray));
            this.Text.AttributedText = text;
        }
    }
}
