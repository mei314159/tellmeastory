using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class TribesListCell : UITableViewCell
    {
        private UIImage defaultPicture;
        public static readonly NSString Key = new NSString("TribesListCell");
        public static readonly UINib Nib;

        static TribesListCell()
        {
            Nib = UINib.FromName("TribesListCell", NSBundle.MainBundle);
        }

        protected TribesListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        private TribeDTO tribe;
        public TribeDTO Tribe
        {
            get
            {
                return tribe;
            }
            set
            {
                tribe = value;
                this.Initialize();
            }
        }

        private void Initialize()
        {
            this.Name.Text = Tribe.Name;
            this.ProfilePicture.SetPictureUrl(null, defaultPicture);
            this.MembershipStatus.Text = Tribe.MembershipStatus.ToString(); //TODO Localize
            //this.AccessoryView = 
            //if (Contact.IsAppUser){
            //    this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            //}
        }
    }
}
