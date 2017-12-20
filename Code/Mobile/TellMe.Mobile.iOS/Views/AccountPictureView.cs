using System;
using CoreGraphics;
using Foundation;
using TellMe.iOS.Core.UI;
using UIKit;

namespace TellMe.iOS.Views
{
    public partial class AccountPictureView : UIView
    {
        public event Action<Picture> OnPictureTouched;

        public AccountPictureView(NSCoder coder) : base(coder)
        {
        }

        public AccountPictureView(NSObjectFlag t) : base(t)
        {
        }

        public AccountPictureView(IntPtr handle) : base(handle)
        {
        }

        public AccountPictureView(CGRect frame) : base(frame)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.PictureTouchWrapper.AddGestureRecognizer(new UITapGestureRecognizer(PictureTouched));
            EditButtonHidden = true;
        }

        public static AccountPictureView Create(CGRect frame)
        {
            var arr = NSBundle.MainBundle.LoadNib("AccountPictureView", null, null);
            var v = ObjCRuntime.Runtime.GetNSObject<AccountPictureView>(arr.ValueAt(0));
            v.Frame = frame;
            return v;
        }

        public Picture PictureView => this.ProfilePicture;

        public bool EditButtonHidden
        {
            get => PictureTouchWrapper.Hidden;
            set => PictureTouchWrapper.Hidden = value;
        }

        public void SetPictureUrl(string pictureUrl, UIImage defaultImage)
        {
            this.ProfilePicture.SetPictureUrl(pictureUrl, defaultImage);
        }

        public void SetPicture(UIImage image)
        {
            this.ProfilePicture.Image = image;
        }

        void PictureTouched()
        {
            this.OnPictureTouched?.Invoke(this.ProfilePicture);
        }
    }
}