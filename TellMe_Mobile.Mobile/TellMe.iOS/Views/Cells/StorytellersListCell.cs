using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class StorytellersListCell : UITableViewCell
    {
        private UIImage defaultPicture;
        public static readonly NSString Key = new NSString("StorytellersListCell");
        public static readonly UINib Nib;

        static StorytellersListCell()
        {
            Nib = UINib.FromName("StorytellersListCell", NSBundle.MainBundle);
        }

        protected StorytellersListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        private StorytellerDTO storyteller;
        public StorytellerDTO Storyteller
        {
            get
            {
                return storyteller;
            }
            set
            {
                storyteller = value;
                this.Initialize();
            }
        }

        private void Initialize()
        {
            this.Username.Text = Storyteller.UserName;
            this.FullName.Text = Storyteller.FullName;
            this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl, defaultPicture);

            if (storyteller.FriendshipStatus == FriendshipStatus.Accepted)
            {
                this.FriendshipStatusLabel.Text = "Friend";
                this.FriendshipStatusLabel.TextColor = UIColor.DarkGray;
            }
            else if (storyteller.FriendshipStatus == FriendshipStatus.Requested)
            {
                this.FriendshipStatusLabel.Text = "Waiting for response";
                this.FriendshipStatusLabel.TextColor = UIColor.LightGray;
            }
            else if (storyteller.FriendshipStatus == FriendshipStatus.WaitingForResponse)
            {
                this.FriendshipStatusLabel.Text = "Followed you";
                this.FriendshipStatusLabel.TextColor = UIColor.Blue;
            }
            else
            {
                this.FriendshipStatusLabel.Text = "Follow";
                this.FriendshipStatusLabel.TextColor = UIColor.Orange;
            }
            //this.AccessoryView = 
            //if (Contact.IsAppUser){
            //    this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            //}
        }
    }
}
