using System;
using Foundation;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Types.Extensions;
using TellMe.iOS.Core.UI;
using UIKit;

namespace TellMe.iOS.Views
{
    public partial class CommentView : UIView
    {
        private CommentDTO _comment;
        private UIImage _defaultPicture;
        public CommentView (IntPtr handle) : base (handle)
        {
        }

        public Action<CommentDTO> ReceiverSelected;
        public Action<CommentDTO> ReplyButtonTouched;

        public CommentDTO Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                this.Initialize();
            }
        }
        
        public static CommentView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("CommentView", null, null);
            var v = ObjCRuntime.Runtime.GetNSObject<CommentView>(arr.ValueAt(0));
            return v;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


            this._defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
        }

        partial void ReplyButton_TouchUpInside(Button sender)
        {
            this.ReplyButtonTouched?.Invoke(Comment);
        }

        private void ProfilePictureTouched()
        {
            this.ReceiverSelected?.Invoke(Comment);
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Comment.AuthorPictureUrl, _defaultPicture);
            this.Time.Text = Comment.CreateDateUtc.GetDateString() + " ago";
            this.UserName.Text = Comment.AuthorUserName;
            this.Text.Text = Comment.Text;
            ReplyButton.Hidden = Comment.ReplyToCommentId != null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _defaultPicture.Dispose();
                _defaultPicture = null;
            }

            base.Dispose(disposing);
        }
    }
}