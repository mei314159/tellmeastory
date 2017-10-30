using System;
using CoreGraphics;
using UIKit;

namespace TellMe.iOS.Views
{
    public class Overlay : CustomPopUpView
    {
        private UIActivityIndicatorView spinner;

        public Overlay(string labelText) : base(new CGSize(200, 250), false)
        {
            this.Layer.CornerRadius = 20;
            this.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
            this.spinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            nfloat spinnerX = (this.Frame.Width - spinner.Frame.Width) / 2;
            nfloat spinnerY = ((labelText != null ? this.Frame.Width : this.Frame.Height) - spinner.Frame.Height) / 2;
            spinner.Frame = new CGRect(spinnerX, spinnerY, spinner.Frame.Width, spinner.Frame.Height);
            this.AddSubview(spinner);
            if (labelText != null)
            {
                nfloat labelY = spinnerY * 2 + spinner.Frame.Height;
                var label = new UILabel(new CGRect(20, labelY, this.Frame.Width - 40, this.Frame.Height - labelY));
                label.Lines = 0;
                label.AdjustsFontSizeToFitWidth = true;
                label.TextColor = UIColor.White;
                label.Text = labelText;
                label.TextAlignment = UITextAlignment.Center;
                this.AddSubview(label);
            }
        }

        public override void PopUp(bool animated = true, Action popAnimationFinish = null)
        {
            InvokeOnMainThread(() =>
            {
                this.spinner.StartAnimating();
                base.PopUp(animated, popAnimationFinish);
            });
        }

        public override void Close(bool animated = true)
        {
            InvokeOnMainThread(() =>
            {
                this.spinner.StopAnimating();
                base.Close(animated);
            });
        }
    }
}
