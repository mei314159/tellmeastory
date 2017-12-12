using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TellMe.iOS.Views
{
    public partial class InputPopupView : CustomPopUpView
    {
        public static readonly NSString Key = new NSString("InputPopupView");
        public static readonly UINib Nib;

        public delegate void OnSubmitHandler(string email);

        public event OnSubmitHandler OnSubmit;

        private InputPopupView(CGSize size, bool showCloseButton) : base(size, showCloseButton)
        {
        }

        static InputPopupView()
        {
            Nib = UINib.FromName("InputPopupView", NSBundle.MainBundle);
        }

        protected InputPopupView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public UIKeyboardType KeyboardType
        {
            get => this.Input.KeyboardType;
            set => this.Input.KeyboardType = value;
        }

        public static InputPopupView Create(string title, string label, string placeholder)
        {
            var view = (InputPopupView) Nib.Instantiate(null, null)[0];
            view.Init(view.Frame.Size, false);
            view.Label.Text = label;
            view.Title.Text = title;
            view.Input.Placeholder = placeholder;
            return view;
        }

        partial void CancelButton_TouchUpInside(UIButton sender)
        {
            Close(true);
        }

        partial void OkButton_TouchUpInside(UIButton sender)
        {
            OnSubmit?.Invoke(this.Input.Text);
            Close(true);
        }
    }
}