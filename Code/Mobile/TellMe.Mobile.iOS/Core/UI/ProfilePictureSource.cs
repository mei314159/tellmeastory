using System;
using UIKit;

namespace TellMe.iOS.Core.UI
{
    public interface IProfilePictureSourceDelegate
    {
        UIView View { get; }
        void PresentModalViewController(UIViewController modalViewController, bool animated);
        void ImageSelected(UIImage image);
    }
    
    public class ProfilePictureSource
    {
        private readonly IProfilePictureSourceDelegate _profilePictureSourceDelegate;

        public ProfilePictureSource(IProfilePictureSourceDelegate profilePictureSourceProfilePictureSourceDelegate)
        {
            _profilePictureSourceDelegate = profilePictureSourceProfilePictureSourceDelegate;
        }
        
        public void ShowPictureSourceDialog()
        {
            var photoSourceActionSheet = new UIActionSheet("Take a photo from");
            photoSourceActionSheet.AddButton("Camera");
            photoSourceActionSheet.AddButton("Photo Library");
            photoSourceActionSheet.AddButton("Cancel");
            photoSourceActionSheet.CancelButtonIndex = 2;
            photoSourceActionSheet.Clicked += PhotoSouceActionSheet_Clicked;
            photoSourceActionSheet.ShowInView(_profilePictureSourceDelegate.View);
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

            _profilePictureSourceDelegate.PresentModalViewController(imagePicker, true);
        }
        
        private void Handle_Canceled(object sender, EventArgs e)
        {
            ((UIImagePickerController) sender).DismissModalViewController(true);
        }
        
        private void Handle_FinishedPickingMediaAsync(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            if (e.Info[UIImagePickerController.MediaType].ToString() != MobileCoreServices.UTType.Image)
                return;

            if (e.Info[UIImagePickerController.OriginalImage] is UIImage originalImage)
            {
                var image = UIImage.FromImage(originalImage.CGImage, 4, originalImage.Orientation);
                _profilePictureSourceDelegate.ImageSelected(image);
            }

            ((UIImagePickerController) sender).DismissModalViewController(true);
        }
    }
}
