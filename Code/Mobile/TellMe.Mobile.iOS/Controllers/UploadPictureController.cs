using System;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.UI.Components;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class UploadPictureController : UIViewController, IUploadPictureView, IProfilePictureSourceDelegate
    {
        private IUploadPictureBusinessLogic _businessLogic;
        private ProfilePictureSource _profilePictureSource;

        public IPicture ProfilePicture => this.Picture;

        public UploadPictureController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            this._businessLogic = IoC.GetInstance<IUploadPictureBusinessLogic>();
            _businessLogic.View = this;
            this._profilePictureSource = new ProfilePictureSource(this);
            View.LayoutIfNeeded();
            Picture.Layer.CornerRadius = Picture.Frame.Width / 2;
            Picture.UserInteractionEnabled = true;
            Picture.AddGestureRecognizer(new UITapGestureRecognizer(_businessLogic.SelectPictureTouched));
            _businessLogic.Init();
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        async partial void ContinueButton_TouchUpInside(UIButton sender)
        {
            await _businessLogic.PictureSelectedAsync();
        }

        partial void SkipButton_TouchUpInside(UIButton sender)
        {
            _businessLogic.SkipButtonTouched();
        }

        public void ShowPictureSourceDialog() => _profilePictureSource.ShowPictureSourceDialog();

        public void ImageSelected(UIImage image)
        {
            Picture.Image = image;
            UIView.Animate(0.2,
                () =>
                {
                    ContinueButton.Enabled = true;
                    ContinueButton.BackgroundColor = UIColor.Blue;
                }, null);
        }
    }
}