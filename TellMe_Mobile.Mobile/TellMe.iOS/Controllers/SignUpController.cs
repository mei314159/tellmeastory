using System;
using CoreGraphics;
using Foundation;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class SignUpController : UIViewController, ISignUpView
    {
        private NSObject _willHideNotificationObserver;
        private NSObject _willShowNotificationObserver;

        private ISignupBusinessLogic _businessLogic;

        public ITextInput UserNameField => this.UserName;

        public ITextInput FullNameField => this.FullName;

        public ITextInput ConfirmPasswordField => this.ConfirmPassword;

        public ITextInput EmailField => this.Email;

        public ITextInput PasswordField => this.Password;

        public SignUpController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this._businessLogic = IoC.GetInstance<ISignupBusinessLogic>();
            _businessLogic.View = this;
            UserName.ShouldReturn += TextFieldShouldReturn;
            Email.ShouldReturn += TextFieldShouldReturn;
            FullName.ShouldReturn += TextFieldShouldReturn;
            Password.ShouldReturn += TextFieldShouldReturn;
            ConfirmPassword.ShouldReturn += TextFieldShouldReturn;
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
            if (_willHideNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willHideNotificationObserver);
            if (_willShowNotificationObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_willShowNotificationObserver);
        }

        async partial void ContinueButton_TouchUpInside(UIButton sender)
        {
            var overlay = new Overlay("Signup in progress");
            overlay.PopUp(true);
            await _businessLogic.SignUpAsync();
            overlay.Close(true);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

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
                ContinueButton_TouchUpInside(ContinueButton);
            }

            return false; // We do not want UITextField to insert line-breaks.
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