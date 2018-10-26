using System;
using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;
using SDWebImage;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.iOS.Views.Cells
{
    public partial class NotificationCenterPlaylistCell : UITableViewCell, INotificationCenterCell
    {
        public static readonly NSString Key = new NSString("NotificationCenterPlaylistCell");
        public static readonly UINib Nib;

        static NotificationCenterPlaylistCell()
        {
            Nib = UINib.FromName("NotificationCenterPlaylistCell", NSBundle.MainBundle);
        }

        protected NotificationCenterPlaylistCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        private NotificationDTO _notification;

        public NotificationDTO Notification
        {
            get => _notification;

            set
            {
                _notification = value;
                Initialize();
            }
        }


        private void Initialize()
        {
            if (Notification.Type != NotificationTypeEnum.SharePlaylist)
            {
                return;
            }

            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString(Notification.Text));

            var playlistDTO = ((JObject) _notification.Extra).ToObject<PlaylistDTO>();
            nint index = 0;
            nint length = 0;
            if (!string.IsNullOrEmpty(playlistDTO.AuthorUserName))
            {
                index = Notification.Text.IndexOf(playlistDTO.AuthorUserName, StringComparison.Ordinal);
                length = playlistDTO.AuthorUserName.Length;
            }
            if (!string.IsNullOrWhiteSpace(playlistDTO.AuthorPictureUrl))
                PictureView.SetImage(new NSUrl(playlistDTO.AuthorPictureUrl));

            if (index >= 0 && length > 0)
            {
                text.AddAttribute(UIStringAttributeKey.Font,
                    UIFont.BoldSystemFontOfSize(this.Text.Font.PointSize),
                    new NSRange(index, length));
            }

            text.Append(new NSAttributedString(" " + Notification.Date.GetDateString(),
                foregroundColor: UIColor.LightGray));
            this.Text.AttributedText = text;
        }
    }
}