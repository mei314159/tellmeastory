using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DataServices.Remote;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using TellMe.Mobile.Core.Types.Extensions;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class StoryViewController : UIViewController, IView, IUITableViewDataSource, IUITableViewDelegate, IStoryView
    {
        private NSObject _willHideNotificationObserver;
        private NSObject _willShowNotificationObserver;

        private readonly List<CommentDTO> _commentsList = new List<CommentDTO>();
        private IRemoteCommentsDataService _commentsService;
        private IRemoteStoriesDataService _remoteStoriesService;
        private ILocalAccountService _localAccountService;
        private IRouter _router;
        private IStoryBusinessLogic _storiesTableBusinessLogic;
        private StoryViewCell _storyView;
        private LoadMoreButtonCell _loadMoreButton;
        private CGRect _originalImageFrame;
        private bool _showLoadMoreButton;
        private CommentDTO _replyToComent;

        public StoryViewController(IntPtr handle) : base(handle)
        {
        }

        private int CommentCellOffset => _showLoadMoreButton ? 2 : 1;

        public StoryDTO Story { get; set; }
        public IView Parent { get; set; }
        public bool DisplayCommentsWhenAppear { get; set; }
        public int StoryId { get; set; }

        public override async void ViewDidLoad()
        {
            await Initialize();
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;

            _commentsService = IoC.GetInstance<IRemoteCommentsDataService>();
            _remoteStoriesService = IoC.GetInstance<IRemoteStoriesDataService>();
            _localAccountService = IoC.GetInstance<ILocalAccountService>();
            _router = IoC.GetInstance<IRouter>();
            _storiesTableBusinessLogic = IoC.GetInstance<IStoryBusinessLogic>();
            _storiesTableBusinessLogic.View = this;
            var swipeGestureRecognizer = new UIPanGestureRecognizer(HandleAction)
            {
                CancelsTouchesInView = false,
                ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => true
            };
            swipeGestureRecognizer.ShouldRecognizeSimultaneously += (gestureRecognizer, otherGestureRecognizer) => true;
            this.View.AddGestureRecognizer(swipeGestureRecognizer);
            NewCommentText.ShouldChangeText += NewCommentText_ShouldChangeText;
            NewCommentText.Changed += NewCommentText_Changed;
            TableView.DataSource = this;
            TableView.Delegate = this;
            TableView.TableFooterView = new UIView();
            TableView.AllowsSelection = false;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.RegisterNibForCellReuse(CommentViewCell.Nib, CommentViewCell.Key);
            TableView.DelaysContentTouches = false;
            SetReplyToBlock(null);
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard)
            {
                CancelsTouchesInView = false
            });
            
            this.LoadCommentsAsync();
        }

        public override void ViewDidAppear(bool animated)
        {
            this._storyView.Play();
            RegisterForKeyboardNotifications();
        }

        public override void ViewWillDisappear(bool animated)
        {
            this._storyView.StopPlaying();

            if (_willHideNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willHideNotificationObserver);
            if (_willShowNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willShowNotificationObserver);
        }

        protected virtual void RegisterForKeyboardNotifications()
        {
            this._willHideNotificationObserver =
                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            this._willShowNotificationObserver =
                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public void OnKeyboardNotification(NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            //Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            //Pass the notification, calculating keyboard height, etc.
            var keyboardFrame = visible
                ? UIKeyboard.FrameEndFromNotification(notification)
                : UIKeyboard.FrameBeginFromNotification(notification);
            OnKeyboardChanged(visible, keyboardFrame);
            //Commit the animation
            UIView.CommitAnimations();
        }

        public virtual void OnKeyboardChanged(bool visible, CGRect keyboardFrame)
        {
            if (View.Superview == null)
            {
                return;
            }

            if (visible)
            {
                BottomOffset.Constant = keyboardFrame.Height;
                TableView.ContentOffset = new CGPoint(0, TableView.ContentSize.Height);
                _storyView.StopPlaying();
            }
            else
            {
                BottomOffset.Constant = 0;
                _storyView.Play();
            }
        }

        private void HandleAction(UIPanGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began)
            {
                _originalImageFrame = View.Frame;
            }

            // Move the image if the gesture is valid
            if (recognizer.State != UIGestureRecognizerState.Cancelled &&
                recognizer.State != UIGestureRecognizerState.Failed &&
                recognizer.State != UIGestureRecognizerState.Possible)
            {
                // Move the image by adding the offset to the object's frame
                var offset = recognizer.TranslationInView(View);
                if (offset.Y < 0 || TableView.ContentOffset.Y > 0)
                {
                    View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
                    return;
                }

                var newFrame = _originalImageFrame;
                newFrame.Offset(0, offset.Y);
                View.Frame = newFrame;
                if ((offset.Y > 150 || recognizer.State == UIGestureRecognizerState.Ended))
                {
                    this.DismissViewController(true, null);
                }
            }
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _storyView.StopPlaying();
                _storyView.Dispose();
            }

            base.Dispose(disposing);
        }

        async partial void SendButtonTouched(NSObject sender)
        {
            SendButton.Enabled = false;
            var text = this.NewCommentText.Text;
            this.NewCommentText.Text = null;
            NewCommentText_Changed(this.NewCommentText, null);

            var authInfo = _localAccountService.GetAuthInfo();
            var comment = new CommentDTO
            {
                Text = text,
                AuthorId = authInfo.Account.Id,
                AuthorUserName = authInfo.Account.UserName,
                AuthorPictureUrl = authInfo.Account.PictureUrl,
                StoryId = Story.Id,
                ReplyToCommentId = _replyToComent?.Id,
                CreateDateUtc = DateTime.UtcNow
            };
            if (_replyToComent == null)
            {
                DisplayComments(true, comment);
            }
            else
            {
                _replyToComent.RepliesCount++;
                DisplayReplies(true, _replyToComent, comment);
            }

            SetReplyToBlock(null);
            SendButton.Enabled = true;

            var result = await _commentsService.AddCommentAsync(comment).ConfigureAwait(false);
            InvokeOnMainThread(() =>
            {
                UpdateComments(comment, result);
            });
        }

        partial void CancelButtonTouched(Button sender, UIEvent @event)
        {
            SetReplyToBlock(null);
        }

        private void UpdateComments(CommentDTO comment, Result<CommentDTO> result)
        {
            if (!result.IsSuccess)
            {
                DeleteComment(comment);
                return;
            }

            comment.Id = result.Data.Id;
            comment.CreateDateUtc = result.Data.CreateDateUtc;
        }

        private void DeleteComment(CommentDTO comment)
        {
            if (comment.ReplyToCommentId == null)
            {
                var index = _commentsList.IndexOf(comment);
                if (index > -1)
                {
                    _commentsList.RemoveAt(index);
                    TableView.DeleteRows(new[] { NSIndexPath.FromRowSection(index + CommentCellOffset, 0) },
                        UITableViewRowAnimation.Automatic);
                }
            }
            else
            {
                var index = _commentsList.IndexOf(x => x.Id == comment.ReplyToCommentId);
                if (index > -1)
                {
                    var cell = (CommentViewCell)TableView.CellAt(NSIndexPath.FromRowSection(index + CommentCellOffset, 0));
                    cell.DeleteComment(comment);
                    cell.Comment.RepliesCount--;
                }
            }
        }

        private async Task LoadCommentsAsync(DateTime? olderThanUtc = null)
        {
            if (_loadMoreButton != null)
                _loadMoreButton.Enabled = false;
            var result = await _commentsService.GetCommentsAsync(Story.Id, olderThanUtc).ConfigureAwait(false);
            if (_loadMoreButton != null)
                _loadMoreButton.Enabled = true;
            if (result.IsSuccess)
            {
                this._showLoadMoreButton = result.Data.TotalCount > _commentsList.Count + result.Data.Items.Length;

                DisplayComments(false, result.Data.Items);
            }
            else
            {
                result.ShowResultError(this);
            }
        }

        private void DisplayReplies(bool addToHead, CommentDTO replyToComment, params CommentDTO[] comments)
        {
            if (comments == null || comments.Length == 0)
            {
                return;
            }

            var index = _commentsList.IndexOf(replyToComment);
            var replyRowIndexPath = NSIndexPath.FromRowSection(index + CommentCellOffset, 0);
            var cell = (CommentViewCell)TableView.CellAt(replyRowIndexPath);
            cell.AddComments(addToHead, comments);
            TableView.ReloadRows(new[] { replyRowIndexPath }, UITableViewRowAnimation.Automatic);
            TableView.LayoutSubviews();
        }

        private void DisplayComments(bool addToHead, params CommentDTO[] comments)
        {
            if (comments == null || comments.Length == 0)
            {
                return;
            }

            var scrollToComments = _commentsList.Count > 0 || DisplayCommentsWhenAppear && comments.Length > 0;
            if (DisplayCommentsWhenAppear)
            {
                DisplayCommentsWhenAppear = false;
            }

            if (addToHead)
                _commentsList.AddRange(comments.OrderBy(x => x.CreateDateUtc));
            else
                _commentsList.InsertRange(0, comments.OrderBy(x => x.CreateDateUtc));


            InvokeOnMainThread(() =>
            {
                TableView.ReloadData();
                if (!scrollToComments)
                    return;

                var index = addToHead
                    ? _commentsList.Count - comments.Length - 1 + CommentCellOffset
                    : this.CommentCellOffset;
                TableView.ScrollToRow(NSIndexPath.FromRowSection(index, 0), UITableViewScrollPosition.Top, true);
            });
        }

        private void NewCommentText_Changed(object sender, EventArgs e)
        {
            var textView = (UITextView)sender;
            var frame = textView.SizeThatFits(new CGSize(textView.Frame.Width, nfloat.MaxValue));
            var newHeight = frame.Height > 100 ? 100 : frame.Height;
            var delta = newHeight - textView.Frame.Height;
            TableView.SetContentOffset(new CGPoint(0, TableView.ContentOffset.Y + delta), false);
            textView.Frame = new CGRect(textView.Frame.Location, new CGSize(textView.Frame.Width, newHeight));
            NewCommentHeight.Constant = textView.Frame.Height;
            View.UpdateConstraints();
        }

        private bool NewCommentText_ShouldChangeText(UITextView textView, NSRange range, string replacementString)
        {
            var text = textView.Text;
            var newText = text.Substring(0, (int)range.Location) + replacementString +
                          text.Substring((int)(range.Location + range.Length));

            return newText.Length <= 500; //max length
        }

        private async Task Initialize()
        {
            if (Story == null)
            {
                var result = await _remoteStoriesService.GetStoryAsync(this.StoryId).ConfigureAwait(false);
                if (result.IsSuccess)
                {
                    Story = result.Data;
                }
                else
                {
                    ShowErrorMessage("Can't load the story");
                    this.DismissViewController(true, null);
                }
            }
            _storyView = StoryViewCell.Create(Story, true);
            _storyView.OnProfilePictureTouched = StoryView_OnProfilePictureTouched;
            _storyView.OnReceiverSelected = StoryView_OnReceiverSelected;
            _storyView.OnLikeButtonTouched = StoryView_OnLikeButtonTouched;
            _storyView.OnMoreButtonTouched = StoryView_MoreButtonTouched;
        }

        private void StoryView_MoreButtonTouched(StoryDTO story)
        {
            var actionSheet = new UIActionSheet("Options");
            actionSheet.AddButton("Add to Playlist");
            actionSheet.AddButton("Cancel");
            actionSheet.CancelButtonIndex = 1;
            actionSheet.Clicked += (sender, args) =>
            {
                if (args.ButtonIndex == 0)
                {
                    _storiesTableBusinessLogic.AddToPlaylist(story);
                }
            };
            actionSheet.ShowInView(View);
        }

        private async void StoryView_OnLikeButtonTouched(StoryDTO story)
        {
            var liked = story.Liked;
            var likeCount = story.LikesCount;
            story.Liked = !liked;
            story.LikesCount = liked ? likeCount - 1 : likeCount + 1;
            App.Instance.StoryLikeChanged(story);

            var result = liked
                ? await _remoteStoriesService.DislikeAsync(story.Id).ConfigureAwait(false)
                : await _remoteStoriesService.LikeAsync(story.Id).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                story.Liked = liked;
                story.LikesCount = likeCount;
                App.Instance.StoryLikeChanged(story);
            }
        }

        private void StoryView_OnReceiverSelected(StoryReceiverDTO receiver)
        {
            if (receiver.TribeId != null)
            {
                this.DismissViewController(false,
                    () => _router.NavigateTribe(Parent, receiver.TribeId.Value, _storyView.RemoveTribe));
            }
            else
            {
                this.DismissViewController(false, () => _router.NavigateStoryteller(Parent, receiver.UserId));
            }
        }

        private void StoryView_OnProfilePictureTouched(StoryDTO story)
        {
            this.DismissViewController(false, () => _router.NavigateStoryteller(Parent, story.SenderId));
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return _commentsList.Count + CommentCellOffset;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                return this._storyView;
            }

            if (_showLoadMoreButton && indexPath.Row == 1)
            {
                if (_loadMoreButton == null)
                {
                    this._loadMoreButton = LoadMoreButtonCell.Create(
                        async b =>
                        {
                            await this.LoadCommentsAsync(_commentsList.First().CreateDateUtc).ConfigureAwait(false);
                        }, "More comments");
                }

                return this._loadMoreButton;
            }

            var commentCell = (CommentViewCell)TableView.DequeueReusableCell(CommentViewCell.Key, indexPath);
            commentCell.Comment = _commentsList[indexPath.Row - CommentCellOffset];
            commentCell.ReceiverSelected = CommentCell_ReceiverSelected;
            commentCell.ReplyButtonTouched = CommentCell_ReplyButtonTouched;
            commentCell.LoadRepliesButtonTouched = CommentCell_LoadRepliesButtonTouched;
            return commentCell;
        }

        private async void CommentCell_LoadRepliesButtonTouched(CommentDTO comment)
        {
            var olderThanUtc = comment.Replies?.FirstOrDefault()?.CreateDateUtc;
            var result = await _commentsService.GetRepliesAsync(Story.Id, comment.Id, olderThanUtc).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                InvokeOnMainThread(() =>
                {
                    DisplayReplies(false, comment, result.Data.Items);
                });
            }
        }

        private void CommentCell_ReplyButtonTouched(CommentDTO comment)
        {
            SetReplyToBlock(comment);
        }

        private void CommentCell_ReceiverSelected(CommentDTO comment)
        {
            this.DismissViewController(false, () => _router.NavigateStoryteller(Parent, comment.AuthorId));
        }

        private void OnStoryLikeChanged(StoryDTO story)
        {
            if (this.Story.Id == story.Id)
                _storyView.UpdateLikeButton(story);
        }

        private void SetReplyToBlock(CommentDTO comment)
        {
            _replyToComent = comment;
            if (comment == null)
            {
                ReplyToWrapperHeight.Constant = 0;
            }
            else
            {
                ReplyToWrapperHeight.Constant = 18;
                var text = new NSMutableAttributedString();
                text.Append(new NSAttributedString("Reply to: "));
                text.Append(new NSAttributedString(comment.AuthorUserName,
                    UIFont.BoldSystemFontOfSize(ReplyToLabel.Font.PointSize)));
                ReplyToLabel.AttributedText = text;
            }
        }
    }
}