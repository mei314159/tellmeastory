using System;
using Contacts;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class StorytellersListCell : UITableViewCell
    {
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

        public Action<StorytellerDTO> OnSendFriendshiopRequestButtonTouched;

        StorytellerDTO storyteller;
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

        partial void InviteButton_TouchUpInside(UIButton sender)
        {
            OnSendFriendshiopRequestButtonTouched?.Invoke(this.Storyteller);
        }

        private void Initialize()
        {
            this.Username.Text = Storyteller.UserName;
            this.FullName.Text = Storyteller.FullName;
            this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl);

            if (storyteller.FriendshipStatus == FriendshipStatus.Accepted){
                this.InviteButton.SetTitle("Friends", UIControlState.Normal);
                this.InviteButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                this.InviteButton.Enabled = false;
            }
            else if (storyteller.FriendshipStatus == FriendshipStatus.Requested)
            {
                this.InviteButton.SetTitle("Waiting for response", UIControlState.Normal);
                this.InviteButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                this.InviteButton.Enabled = false;
            }
            else if (storyteller.FriendshipStatus == FriendshipStatus.WaitingForResponse)
            {
                this.InviteButton.SetTitle("Accept friendship", UIControlState.Normal);
                this.InviteButton.SetTitleColor(UIColor.LightGray, UIControlState.Normal);
                this.InviteButton.Enabled = true;
            }
            else
            {
                this.InviteButton.SetTitle("Add to friends", UIControlState.Normal);
                this.InviteButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
                this.InviteButton.Enabled = true;
            }

            //if (Contact.IsAppUser){
            //    this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            //}
        }
    }
}
