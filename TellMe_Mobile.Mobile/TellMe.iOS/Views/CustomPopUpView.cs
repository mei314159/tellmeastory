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
                btnClose.TouchUpInside += delegate
                {
                    Close();
                };
                this.AddSubview(btnClose);
            }
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
                UIView.Animate(0.15, delegate
                {
                    Transform = CGAffineTransform.MakeScale(1, 1);
                    effectView.Alpha = 0.8f;
                }, delegate
                {
                    if (null != popAnimationFinish)
                        popAnimationFinish();
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
                UIView.Animate(0.15, delegate
                {
                    Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
                    effectView.Alpha = 0;
                }, delegate
                {
                    this.RemoveFromSuperview();
                    effectView.RemoveFromSuperview();
                    if (null != PopWillClose) PopWillClose();
                });
            }
            else
            {
                if (null != PopWillClose) PopWillClose();
            }
        }
    }
}
