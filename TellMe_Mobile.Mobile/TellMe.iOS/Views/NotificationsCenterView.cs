using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace TellMe.iOS
{
    public partial class NotificationsCenterView : UIView, IUITableViewDelegate, IUITableViewDataSource
    {
        private nfloat Height;
        public delegate void PopWillCloseHandler();
        public event PopWillCloseHandler PopWillClose;

        public static readonly NSString Key = new NSString("NotificationsCenterView");
        public static readonly UINib Nib;

        private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight));

        protected NotificationsCenterView(IntPtr handle) : base(handle)
        {
        }

        static NotificationsCenterView()
        {
            Nib = UINib.FromName("NotificationsCenterView", NSBundle.MainBundle);
        }

        public static NotificationsCenterView Create(RemoteNotificationsDataService remoteNotificationsDataService)
        {
            var view = (NotificationsCenterView)Nib.Instantiate(null, null)[0];
            view.Init(65);
            return view;
        }

        public virtual void PopUp(bool animated = true, Action popAnimationFinish = null)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            effectView.Frame = new CGRect(0, this.Frame.Y, window.Bounds.Width, window.Bounds.Height - this.Frame.Y);
            window.EndEditing(true);
            //window.AddSubview(effectView);
            window.AddSubview(this);

            if (animated)
            {
                Frame = new CGRect(this.Frame.Location, new CGSize(this.Frame.Width, 0));
                UIView.Animate(0.3, delegate
                {
                    Frame = new CGRect(this.Frame.Location, new CGSize(this.Frame.Width, Height));
                    effectView.Alpha = 0.8f;
                }, delegate
                {
                    popAnimationFinish?.Invoke();
                });
            }
            else
            {
                effectView.Alpha = 0.8f;
            }
        }

        public virtual void Close(bool animated = true)
        {
            if (animated)
            {
                UIView.Animate(0.3, delegate
                {
                    Frame = new CGRect(this.Frame.Location, new CGSize(this.Frame.Width, 0));
                    effectView.Alpha = 0;
                }, delegate
                {
                    this.RemoveFromSuperview();
                    effectView.RemoveFromSuperview();
                    PopWillClose?.Invoke();
                });
            }
            else
            {
                PopWillClose?.Invoke();
            }
        }

        protected void Init(nfloat y)
        {
            this.Height = UIScreen.MainScreen.Bounds.Height / 2;
            this.Frame = new CGRect(0, y, UIScreen.MainScreen.Bounds.Width, this.Height);
            effectView.Alpha = 0;
            this.BackgroundColor = UIColor.FromRGB(249, 249, 249);
            this.TableView.TableFooterView = new UIView();
        }
        partial void CloseButton_TouchUpInside(UIButton sender)
        {
            Close(true);
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            throw new NotImplementedException();
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            throw new NotImplementedException();
        }
    }
}