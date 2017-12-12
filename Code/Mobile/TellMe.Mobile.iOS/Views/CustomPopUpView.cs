using System;
using CoreGraphics;
using UIKit;

namespace TellMe.iOS.Views
{
    public class CustomPopUpView : UIView
    {
        public delegate void PopWillCloseHandler();

        public event PopWillCloseHandler PopWillClose;

        private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
        private UIButton btnClose = new UIButton(UIButtonType.System);

        public CustomPopUpView(CGSize size, bool showCloseButton = true)
        {
            Init(size, showCloseButton);
        }

        protected virtual void Init(CGSize size, bool showCloseButton)
        {
            nfloat lx = (UIScreen.MainScreen.Bounds.Width - size.Width) / 2;
            nfloat ly = (UIScreen.MainScreen.Bounds.Height - size.Height) / 2;
            this.Frame = new CGRect(new CGPoint(lx, ly), size);

            effectView.Alpha = 0;

            this.BackgroundColor = UIColor.White;

            if (showCloseButton)
            {
                nfloat btnHeight = 40;
                btnClose.SetTitle("Close", UIControlState.Normal);
                btnClose.Frame = new CGRect(0, this.Frame.Height - btnHeight, this.Frame.Width, btnHeight);
                btnClose.TouchUpInside += (s, e) => Close();
                this.AddSubview(btnClose);
            }
        }

        protected CustomPopUpView(IntPtr handle) : base(handle)
        {
        }

        public virtual void PopUp(bool animated = true, Action popAnimationFinish = null)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            effectView.Frame = window.Bounds;
            window.EndEditing(true);
            window.AddSubview(effectView);
            window.AddSubview(this);

            if (animated)
            {
                Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
                Animate(0.15, delegate
                {
                    Transform = CGAffineTransform.MakeScale(1, 1);
                    effectView.Alpha = 0.8f;
                }, delegate { popAnimationFinish?.Invoke(); });
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
                Animate(0.15, delegate
                {
                    Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
                    effectView.Alpha = 0;
                }, () =>
                {
                    this.RemoveFromSuperview();
                    effectView.RemoveFromSuperview();
                    PopWillClose?.Invoke();
                });
            }
            else
            {
                this.RemoveFromSuperview();
                effectView.RemoveFromSuperview();
                PopWillClose?.Invoke();
            }
        }
    }
}