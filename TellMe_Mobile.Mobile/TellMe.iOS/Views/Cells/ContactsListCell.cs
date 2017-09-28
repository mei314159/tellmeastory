using System;
using Contacts;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class ContactsListCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ContactsListCell");
        public static readonly UINib Nib;

        static ContactsListCell()
        {
            Nib = UINib.FromName("ContactsListCell", NSBundle.MainBundle);
        }

        protected ContactsListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        ContactDTO contact;
        public ContactDTO Contact
        {
            get
            {
                return contact;
            }

            set
            {
                contact = value;
                this.Initialize();
            }
        }

        partial void InviteButton_TouchUpInside(UIButton sender)
        {
            
        }

        private void Initialize()
        {
            this.Name.Text = Contact.Name;
            this.PhoneNumber.Text = Contact.PhoneNumber;
            this.InviteButton.Hidden = Contact.IsAppUser;
        }
    }
}
