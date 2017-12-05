using System;
using Foundation;
using TellMe.Mobile.Core.Contracts.UI.Components;
using UIKit;

namespace TellMe.iOS.Core.UI
{
    [Register("Label")]
    public class Label : UILabel, ITextLabel
    {
        public Label()
        {
        }

        public Label(NSCoder coder) : base(coder)
        {
        }

        public Label(NSObjectFlag t) : base(t)
        {
        }

        public Label(IntPtr handle) : base(handle)
        {
        }

        public Label(CoreGraphics.CGRect frame) : base(frame)
        {
        }
    }
}