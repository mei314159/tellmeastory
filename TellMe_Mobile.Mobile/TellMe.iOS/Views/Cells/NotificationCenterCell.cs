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
    public partial class NotificationCenterCell : UITableViewCell, INotificationCenterCell
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

        private NotificationDTO notification;

        public NotificationDTO Notification
        {
            get { return notification; }

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
                    var storyRequestDTO = ((JObject) notification.Extra).ToObject<StoryRequestDTO>();
                    index = Notification.Text.IndexOf(storyRequestDTO.ReceiverName, StringComparison.Ordinal);
                    length = storyRequestDTO.ReceiverName.Length;
                    if (!string.IsNullOrWhiteSpace(storyRequestDTO.SenderPictureUrl))
                        PictureView.SetImage(new NSUrl(storyRequestDTO.SenderPictureUrl));
                    if (!Notification.Handled)
                    {
                        this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }
                    break;
                case NotificationTypeEnum.FriendshipRequest:
                    var dto = ((JObject) notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    if (!Notification.Handled)
                    {
                        this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }

                    if (!string.IsNullOrWhiteSpace(dto.PictureUrl))
                        PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                case NotificationTypeEnum.FriendshipAccepted:
                    dto = ((JObject) notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    if (!string.IsNullOrWhiteSpace(dto.PictureUrl))
                        PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                case NotificationTypeEnum.FriendshipRejected:
                    dto = ((JObject) notification.Extra).ToObject<StorytellerDTO>();
                    index = Notification.Text.IndexOf(dto.UserName, StringComparison.Ordinal);
                    length = dto.UserName.Length;
                    if (!string.IsNullOrWhiteSpace(dto.PictureUrl))
                        PictureView.SetImage(new NSUrl(dto.PictureUrl));
                    break;
                case NotificationTypeEnum.TribeInvite:
                    var tribeDto = ((JObject) notification.Extra).ToObject<TribeDTO>();
                    index = Notification.Text.IndexOf(tribeDto.CreatorName, StringComparison.Ordinal);
                    length = tribeDto.CreatorName.Length;
                    if (!Notification.Handled)
                    {
                        this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    }
                    if (!string.IsNullOrWhiteSpace(tribeDto.CreatorPictureUrl))
                        PictureView.SetImage(new NSUrl(tribeDto.CreatorPictureUrl));
                    break;
                case NotificationTypeEnum.TribeAcceptInvite:
                    var tribeMemberDto = ((JObject) notification.Extra).ToObject<TribeMemberDTO>();
                    index = Notification.Text.IndexOf(tribeMemberDto.UserName, StringComparison.Ordinal);
                    length = tribeMemberDto.UserName.Length;
                    if (!string.IsNullOrWhiteSpace(tribeMemberDto.UserPictureUrl))
                        PictureView.SetImage(new NSUrl(tribeMemberDto.UserPictureUrl));
                    break;
                case NotificationTypeEnum.TribeRejectInvite:
                    tribeMemberDto = ((JObject) notification.Extra).ToObject<TribeMemberDTO>();
                    index = Notification.Text.IndexOf(tribeMemberDto.UserName, StringComparison.Ordinal);
                    length = tribeMemberDto.UserName.Length;
                    if (!string.IsNullOrWhiteSpace(tribeMemberDto.UserPictureUrl))
                        PictureView.SetImage(new NSUrl(tribeMemberDto.UserPictureUrl));
                    break;
                default:
                    this.Text.AttributedText = text;
                    text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(),
                        foregroundColor: UIColor.LightGray));
                    return;
            }

            text.AddAttribute(UIStringAttributeKey.Font, UIFont.BoldSystemFontOfSize(this.Text.Font.PointSize),
                new NSRange(index, length));
            text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(),
                foregroundColor: UIColor.LightGray));
            this.Text.AttributedText = text;
        }
    }
}