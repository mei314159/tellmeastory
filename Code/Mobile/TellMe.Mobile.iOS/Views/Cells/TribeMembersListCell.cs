using System;
using Foundation;
using TellMe.Mobile.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class TribeMembersListCell : UITableViewCell
    {
        private UIImage defaultPicture;
        public static readonly NSString Key = new NSString("TribeMembersListCell");
        public static readonly UINib Nib;

        static TribeMembersListCell()
        {
            Nib = UINib.FromName("TribeMembersListCell", NSBundle.MainBundle);
        }

        protected TribeMembersListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        private TribeMemberDTO tribeMember;

        public TribeMemberDTO TribeMember
        {
            get { return tribeMember; }
            set
            {
                tribeMember = value;
                this.Initialize();
            }
        }

        private void Initialize()
        {
            this.Username.Text = TribeMember.UserName;
            this.FullName.Text = TribeMember.FullName;
            this.ProfilePicture.SetPictureUrl(TribeMember.UserPictureUrl, defaultPicture);
            this.StatusLabel.Text = TribeMember.Status.ToString();
        }
    }
}