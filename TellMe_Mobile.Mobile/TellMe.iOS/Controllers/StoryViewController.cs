using System;
using CoreGraphics;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.DTO;
using UIKit;
using TellMe.Core.Types.Extensions;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views.Cells;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.iOS.Core;

namespace TellMe.iOS
{
    public partial class StoryViewController : UIViewController, IView, IUITableViewDataSource, IUITableViewDelegate
    {
        private NSObject _willHideNotificationObserver;
        private NSObject _willShowNotificationObserver;

        private readonly List<CommentDTO> _commentsList = new List<CommentDTO>();
        private UIVisualEffectView _effectView;

        private UIVisualEffectView EffectView => _effectView ??
                                                 (_effectView =
                                                     new UIVisualEffectView(
                                                         UIBlurEffect.FromStyle(UIBlurEffectStyle.Light)));

        private IRemoteCommentsDataService _commentsService;
        private IRemoteStoriesDataService _remoteStoriesService;
        private ILocalAccountService _localAccountService;
        private IRouter _router;
        private StoryViewCell _storyView;
        private LoadMoreButtonCell _loadMoreButton;

        public StoryViewController(IntPtr handle) : base(handle)
        {
        }

        private CGRect _originalImageFrame;
        private bool _showLoadMoreButton;

        private int CommentCellOffset => _showLoadMoreButton ? 2 : 1;

        public StoryDTO Story { get; set; }
        public IView Parent { get; set; }
        public bool DisplayCommentsWhenAppear { get; set; }

        public override void ViewDidLoad()
        {
            Initialize();
            EffectView.Frame = View.Bounds;
            View.AddSubview(EffectView);
            View.SendSubviewToBack(EffectView);
            App.Instance.OnStoryLikeChanged += OnStoryLikeChanged;

            _commentsService = IoC.Container.GetInstance<IRemoteCommentsDataService>();
            _remoteStoriesService = IoC.Container.GetInstance<IRemoteStoriesDataService>();
            _localAccountService = IoC.Container.GetInstance<ILocalAccountService>();
            _router = IoC.Container.GetInstance<IRouter>();
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
            UIView.SetAnimationCurve((UIViewAnimationCurve) UIKeyboard.AnimationCurveFromNotification(notification));

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

            var authInfo = _localAccountService.GetAuthInfo();
            var comment = new CommentDTO
            {
                Text = text,
                AuthorId = authInfo.Account.Id,
                AuthorUserName = authInfo.Account.UserName,
                AuthorPictureUrl = authInfo.Account.PictureUrl,
                StoryId = Story.Id,
                CreateDateUtc = DateTime.UtcNow
            };

            DisplayComments(true, comment);
            SendButton.Enabled = true;

            var result = await _commentsService.AddCommentAsync(Story.Id, text).ConfigureAwait(false);

            InvokeOnMainThread(() =>
            {
                if (result.IsSuccess)
                {
                    UpdateComments(comment, result.Data);
                }
                else
                {
                    DeleteComment(comment);
                }
            });
        }

        private void UpdateComments(CommentDTO comment, CommentDTO newComment)
        {
            var index = _commentsList.IndexOf(comment);
            if (index > -1)
            {
                comment.Id = newComment.Id;
                comment.CreateDateUtc = newComment.CreateDateUtc;

                var indexPath = NSIndexPath.FromRowSection(index + CommentCellOffset, 0);
                if (TableView.IndexPathsForVisibleRows.Any(
                    x => x.Row == indexPath.Row && x.Section == indexPath.Section))
                {
                    TableView.ReloadRows(new[] {indexPath}, UITableViewRowAnimation.None);
                }
            }
        }

        private void DeleteComment(CommentDTO comment)
        {
            var index = _commentsList.IndexOf(comment);
            if (index > -1)
            {
                _commentsList.RemoveAt(index);
                TableView.DeleteRows(new[] {NSIndexPath.FromRowSection(index + CommentCellOffset, 0)},
                    UITableViewRowAnimation.Automatic);
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

        private void DisplayComments(bool addToHead = false, params CommentDTO[] comments)
        {
            if (comments == null || comments.Length == 0)
            {
                return;
            }
            
            var scrollToComments = _commentsList.Count > 0 || (DisplayCommentsWhenAppear && comments?.Length > 0);
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
                if (scrollToComments)
                {
                    var index = addToHead
                        ? _commentsList.Count - comments.Length - 1 + CommentCellOffset
                        : (this.CommentCellOffset);
                    TableView.ScrollToRow(NSIndexPath.FromRowSection(index, 0), UITableViewScrollPosition.Top, true);
                }
            });
        }

        private void NewCommentText_Changed(object sender, EventArgs e)
        {
            var textView = (UITextView) sender;
            var frame = textView.SizeThatFits(new CGSize(textView.Frame.Width, nfloat.MaxValue));
            nfloat newHeight = frame.Height > 100 ? 100 : frame.Height;
            var delta = newHeight - textView.Frame.Height;
            TableView.SetContentOffset(new CGPoint(0, TableView.ContentOffset.Y + delta), false);
            textView.Frame = new CGRect(textView.Frame.Location, new CGSize(textView.Frame.Width, newHeight));
            NewCommentWrapperHeight.Constant = textView.Frame.Height + 10;
            View.UpdateConstraints();
        }

        private bool NewCommentText_ShouldChangeText(UITextView textView, NSRange range, string replacementString)
        {
            string text = textView.Text;
            string newText = text.Substring(0, (int) range.Location) + replacementString +
                             text.Substring((int) (range.Location + range.Length));

            return newText.Length <= 500; //max length
        }

        private void Initialize()
        {
            _storyView = StoryViewCell.Create(Story);
            _storyView.OnProfilePictureTouched = StoryView_OnProfilePictureTouched;
            _storyView.OnReceiverSelected = StoryView_OnReceiverSelected;
            _storyView.OnLikeButtonTouched = StoryView_OnLikeButtonTouched;
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
                        async (b) =>
                        {
                            await this.LoadCommentsAsync(_commentsList.First().CreateDateUtc).ConfigureAwait(false);
                        }, "More comments");
                }

                return this._loadMoreButton;
            }

            var commentCell = TableView.DequeueReusableCell(CommentViewCell.Key, indexPath) as CommentViewCell;
            commentCell.Comment = _commentsList[indexPath.Row - CommentCellOffset];
            commentCell.ReceiverSelected = CommentCell_ReceiverSelected;
            return commentCell;
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
    }
}