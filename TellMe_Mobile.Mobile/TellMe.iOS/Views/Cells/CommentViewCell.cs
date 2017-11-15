using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;

namespace TellMe.iOS.Views.Cells
{
    public partial class CommentViewCell : UITableViewCell
    {
        private CommentDTO comment;
        private UIImage defaultPicture;
        public static readonly NSString Key = new NSString("CommentViewCell");
        public static readonly UINib Nib;

        static CommentViewCell()
        {
            Nib = UINib.FromName("CommentViewCell", NSBundle.MainBundle);
        }

        protected CommentViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public Action<CommentDTO> ReceiverSelected;

        public CommentDTO Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                this.Initialize();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


            this.defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Comment.AuthorPictureUrl, defaultPicture);
            this.Time.Text = Comment.CreateDateUtc.GetDateString() + " ago";
            this.UserName.Text = Comment.AuthorUserName;
            this.Text.Text = Comment.Text;
        }

        private void ProfilePictureTouched()
        {
            this.ReceiverSelected?.Invoke(Comment);
        }
    }
}