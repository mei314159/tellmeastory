using System;
using Foundation;
using TellMe.Core.Contracts.UI.Components;
using TellMe.iOS.Core.UI;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class LoadMoreButtonCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("LoadMoreButtonCell");
        public static readonly UINib Nib;
        private volatile bool enabled;

        static LoadMoreButtonCell()
        {
            Nib = UINib.FromName("LoadMoreButtonCell", NSBundle.MainBundle);
        }

        protected LoadMoreButtonCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                InvokeOnMainThread(() =>
                {
                    if (value)
                    {
                        Spinner.StopAnimating();
                    }
                    else
                    {
                        Spinner.StartAnimating();
                    }
                    Button.Enabled = value;
                    Button.Hidden = !value;
                });
            }
        }

        public event Action<IButton> OnTouched;

        public static LoadMoreButtonCell Create(Action<IButton> callback, string text = null)
        {
            var arr = NSBundle.MainBundle.LoadNib("LoadMoreButtonCell", null, null);
            var v = ObjCRuntime.Runtime.GetNSObject<LoadMoreButtonCell>(arr.ValueAt(0));
            v.OnTouched += callback;
            if (text != null)
            {
                v.Button.SetTitle(text, UIControlState.Normal);
            }

            return v;
        }

        partial void Button_TouchUpInside(Button sender)
        {
            OnTouched?.Invoke(sender);
        }
    }
}