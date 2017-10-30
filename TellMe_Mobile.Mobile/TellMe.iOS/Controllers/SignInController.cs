using Foundation;
using System;
using UIKit;
using TellMe.Core;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Types.DataServices.Local;
using TellMe.iOS.Extensions;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using CoreGraphics;
using TellMe.iOS.Views;

namespace TellMe.iOS
{
    public partial class SignInController : UIViewController, ISignInView
    {
        private NSObject willHideNotificationObserver;
        private NSObject willShowNotificationObserver;

        private SigninBusinessLogic _businessLogic;

        public ITextInput EmailField => this.Email;

        public ITextInput PasswordField => this.Password;

        public SignInController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = new SigninBusinessLogic(
                App.Instance.Router,
                new RemoteAccountDataService(),
                new AccountService(),
                this);
            Email.ShouldReturn += TextFieldShouldReturn;
            Password.ShouldReturn += TextFieldShouldReturn;

            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            this._businessLogic.Init();
        }

        public override void ViewDidAppear(bool animated)
        {
            this.ScrollView.ContentSize = new CGSize(ScrollView.Frame.Width, ScrollView.Frame.Height);
            RegisterForKeyboardNotifications();
        }

        public override void ViewDidDisappear(bool animated)
        {
            if (willHideNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(willHideNotificationObserver);
            if (willShowNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(willShowNotificationObserver);
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        async partial void LogInButton_TouchUpInside(UIButton sender)
        {
            var overlay = new Overlay("Signin in progress");
            overlay.PopUp(true);
            await _businessLogic.SignInAsync();
            overlay.Close(true);
        }

        private bool TextFieldShouldReturn(UITextField textField)
        {
            var nextTag = textField.Tag + 1;
            UIResponder nextResponder = this.View.ViewWithTag(nextTag);
            if (nextResponder != null && nextTag < 2)
            {
                nextResponder.BecomeFirstResponder();
            }
            else
            {
                // Not found, so remove keyboard.
                textField.ResignFirstResponder();
                LogInButton_TouchUpInside(LogInButton);
            }

            return false; // We do not want UITextField to insert line-breaks.
        }

        protected virtual void RegisterForKeyboardNotifications()
        {
            this.willHideNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            this.willShowNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
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
                ScrollViewBottomMargin.Constant = -keyboardFrame.Height;
                this.ScrollView.ContentOffset = new CGPoint(0, 110);
            }
            else
            {
                ScrollViewBottomMargin.Constant = 0;
                this.ScrollView.ContentOffset = new CGPoint(0, 0);
            }
        }
    }
}