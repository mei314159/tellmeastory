using System;
using Foundation;
using TellMe.Mobile.Core.Contracts.UI.Components;
using UIKit;

namespace TellMe.iOS.Core.UI
{
    [Register("Button")]
    public class Button : UIButton, IButton
    {
        public Button()
        {
        }

        public Button(Foundation.NSCoder coder) : base(coder)
        {
        }

        public Button(Foundation.NSObjectFlag t) : base(t)
        {
        }

        public Button(IntPtr handle) : base(handle)
        {
        }

        public Button(CoreGraphics.CGRect frame) : base(frame)
        {
        }

        public string TitleString
        {
            get { return this.TitleLabel.Text; }
            set { this.SetTitle(value, UIControlState.Normal); }
        }
    }
}