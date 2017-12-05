using System;
using CoreGraphics;
using TellMe.Mobile.Core.Contracts.UI;
using UIKit;

namespace TellMe.iOS.Views
{
    public sealed class Overlay : CustomPopUpView, IOverlay
    {
        private readonly UIActivityIndicatorView _spinner;
        private readonly UILabel _uiLabel;

        public Overlay(string labelText) : base(new CGSize(200, 250), false)
        {
            this.Layer.CornerRadius = 20;
            this.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
            this._spinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            nfloat spinnerX = (this.Frame.Width - _spinner.Frame.Width) / 2;
            nfloat spinnerY = ((labelText != null ? this.Frame.Width : this.Frame.Height) - _spinner.Frame.Height) / 2;
            _spinner.Frame = new CGRect(spinnerX, spinnerY, _spinner.Frame.Width, _spinner.Frame.Height);
            this.AddSubview(_spinner);
            if (labelText != null)
            {
                nfloat labelY = spinnerY * 2 + _spinner.Frame.Height;
                _uiLabel =
                    new UILabel(new CGRect(20, labelY, this.Frame.Width - 40, this.Frame.Height - labelY))
                    {
                        Lines = 0,
                        AdjustsFontSizeToFitWidth = true,
                        TextColor = UIColor.White,
                        Text = labelText,
                        TextAlignment = UITextAlignment.Center
                    };
                this.AddSubview(_uiLabel);
            }
        }

        public string Text
        {
            get => _uiLabel?.Text;
            set => _uiLabel.Text = value;
        }

        public override void PopUp(bool animated = true, Action popAnimationFinish = null)
        {
            InvokeOnMainThread(() =>
            {
                this._spinner.StartAnimating();
                base.PopUp(animated, popAnimationFinish);
            });
        }

        public override void Close(bool animated = true)
        {
            InvokeOnMainThread(() =>
            {
                this._spinner.StopAnimating();
                base.Close(animated);
            });
        }
    }
}