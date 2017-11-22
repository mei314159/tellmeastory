using System;
using System.IO;
using Foundation;
using SDWebImage;
using TellMe.Core.Contracts.UI.Components;
using TellMe.iOS.Extensions;
using UIKit;

namespace TellMe.iOS.Core.UI
{
    [Register("Picture")]
    public class Picture : UIImageView, IPicture
    {
        public Picture()
        {
        }

        public Picture(NSCoder coder) : base(coder)
        {
        }

        public Picture(NSObjectFlag t) : base(t)
        {
        }

        public Picture(IntPtr handle) : base(handle)
        {
        }

        public Picture(CoreGraphics.CGRect frame) : base(frame)
        {
        }

        public Picture(UIImage image) : base(image)
        {
        }

        public Picture(UIImage image, UIImage highlightedImage) : base(image, highlightedImage)
        {
        }

        public Stream GetPictureStream()
        {
            if (this.Image == null)
                return null;

            var stream = this.Image.MaxResizeImage(320, 320).AsJPEG().AsStream();
            stream.Position = 0;
            return stream;
        }

        public void SetPictureUrl(string pictureUrl, object defaultPicture)
        {
            if (pictureUrl != null)
                this.SetImage(new NSUrl(pictureUrl));
            else if (defaultPicture != null && defaultPicture is UIImage)
            {
                var defPic = (UIImage) defaultPicture;
                this.Image = defPic;
            }
        }
    }
}