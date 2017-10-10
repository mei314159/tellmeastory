using System;
using Foundation;
using TellMe.Core.Contracts.UI.Components;
using UIKit;

namespace TellMe.iOS
{
    [Register("TextInput")]
    public class TextInput : UITextField, ITextInput
    {
        public TextInput()
        {
        }

        public TextInput(Foundation.NSCoder coder) : base(coder)
        {
        }

        public TextInput(Foundation.NSObjectFlag t) : base(t)
        {
        }

        public TextInput(IntPtr handle) : base(handle)
        {
        }

        public TextInput(CoreGraphics.CGRect frame) : base(frame)
        {
        }
    }
}