using System;
using System.Linq;
using Foundation;
using UIKit;
using System.Collections.Generic;
using TellMe.iOS.Core.UI;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Types.Extensions;

namespace TellMe.iOS.Views.Cells
{
    public partial class CommentViewCell : UITableViewCell
    {
        private CommentDTO _comment;
        private UIImage _defaultPicture;
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
        public Action<CommentDTO> ReplyButtonTouched;
        public Action<CommentDTO> LoadRepliesButtonTouched;

        public CommentDTO Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                this.Initialize();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


            this._defaultPicture = UIImage.FromBundle("UserPic");

            this.ProfilePicture.UserInteractionEnabled = true;
            this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(this.ProfilePictureTouched));
        }

        private void Initialize()
        {
            this.ProfilePicture.SetPictureUrl(Comment.AuthorPictureUrl, _defaultPicture);
            this.Time.Text = Comment.CreateDateUtc.GetDateString() + " ago";
            this.UserName.Text = Comment.AuthorUserName;
            this.Text.Text = Comment.Text;
            DisplayComments(Comment.Replies);
        }

        public void AddComments(bool addToHead = false, params CommentDTO[] comments)
        {
            if (comments == null || comments.Length == 0)
            {
                return;
            }

            var orderedComments = comments.OrderBy(x => x.CreateDateUtc);
            if (Comment.Replies == null)
                Comment.Replies = new List<CommentDTO>();
            if (addToHead)
                Comment.Replies.AddRange(orderedComments);
            else
                Comment.Replies.InsertRange(0, orderedComments);
            DisplayComments(Comment.Replies);
        }

        private void DisplayComments(ICollection<CommentDTO> comments)
        {
            this.ShowRepliesButton.Hidden = Comment.RepliesCount == 0 || (Comment.RepliesCount <= Comment.Replies?.Count);
            var commentsCount = comments?.Count ?? 0;
            var subviews = Replies.ArrangedSubviews.OfType<CommentView>().ToList();
            subviews.Skip(commentsCount).ToList().ForEach(x =>
            {
                x.RemoveFromSuperview();
                subviews.Remove(x);
            });


            if (commentsCount == 0)
                return;

            for (var i = 0; i < commentsCount; i++)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var comment = comments.ElementAt(i);
                if (i < subviews.Count)
                {
                    subviews[i].Comment = comment;
                }
                else
                {
                    var view = CommentView.Create();
                    view.Comment = comment;
                    Replies.AddArrangedSubview(view);
                }
            }
        }

        public void DeleteComment(CommentDTO comment)
        {
            Comment.Replies.Remove(comment);
        }

        private void ProfilePictureTouched()
        {
            this.ReceiverSelected?.Invoke(Comment);
        }

        partial void ReplyButton_TouchUpInside(Button sender)
        {
            this.ReplyButtonTouched?.Invoke(Comment);
        }

        partial void ShowRepliesButton_TouchUpInside(Button sender)
        {
            this.LoadRepliesButtonTouched?.Invoke(Comment);
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