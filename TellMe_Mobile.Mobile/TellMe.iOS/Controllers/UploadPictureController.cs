using System;
using UIKit;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.UI.Components;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class UploadPictureController : UIViewController, IUploadPictureView
    {
        private IUploadPictureBusinessLogic _businessLogic;

        public IPicture ProfilePicture => this.Picture;

        public UploadPictureController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            this._businessLogic = IoC.Container.GetInstance<IUploadPictureBusinessLogic>();
            _businessLogic.View = this;
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

        public void ShowPictureSourceDialog()
        {
            var photoSourceActionSheet = new UIActionSheet("Take a photo from");
            photoSourceActionSheet.AddButton("Camera");
            photoSourceActionSheet.AddButton("Photo Library");
            photoSourceActionSheet.AddButton("Cancel");
            photoSourceActionSheet.CancelButtonIndex = 2;
            photoSourceActionSheet.Clicked += PhotoSouceActionSheet_Clicked;
            photoSourceActionSheet.ShowInView(View);
        }

        private void PhotoSouceActionSheet_Clicked(object sender, UIButtonEventArgs e)
        {
            var imagePicker = new UIImagePickerController();
            imagePicker.MediaTypes = new string[] {MobileCoreServices.UTType.Image};
            if (e.ButtonIndex == 0)
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            }
            else if (e.ButtonIndex == 1)
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            }
            else
            {
                return;
            }

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMediaAsync;
            imagePicker.Canceled += Handle_Canceled;

            PresentModalViewController(imagePicker, true);
        }

        private void Handle_FinishedPickingMediaAsync(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            if (e.Info[UIImagePickerController.MediaType].ToString() != MobileCoreServices.UTType.Image)
                return;

            if (e.Info[UIImagePickerController.OriginalImage] is UIImage originalImage)
            {
                Picture.Image = UIImage.FromImage(originalImage.CGImage, 4, originalImage.Orientation);
                UIView.Animate(0.2,
                    () =>
                    {
                        ContinueButton.Enabled = true;
                        ContinueButton.BackgroundColor = UIColor.Blue;
                    }, null);
            }

            ((UIImagePickerController) sender).DismissModalViewController(true);
        }

        private void Handle_Canceled(object sender, EventArgs e)
        {
            ((UIImagePickerController) sender).DismissModalViewController(true);
        }
    }
}