using System;
using CoreGraphics;
using Foundation;
using MonoTouch.Dialog;
using SafariServices;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.Mobile.Core;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class ProfileViewController : DialogViewController, IAccountView, IProfilePictureSourceDelegate
    {
        private IAccountBusinessLogic _accountBusinessLogic;
        private UIBarButtonItem _saveButton;
        private UIImage _defaultImage;
        private ProfilePictureSource _profilePictureSource;
        private AccountPictureView _picture;
        public bool PictureChanged { get; private set; }

        public ProfileViewController(IntPtr handle) : base(handle)
        {
        }

        public UserDTO User { get; set; }

        public override void ViewDidLoad()
        {
            _accountBusinessLogic = IoC.GetInstance<IAccountBusinessLogic>();
            _accountBusinessLogic.View = this;
            this._profilePictureSource = new ProfilePictureSource(this);

            this._accountBusinessLogic.InitView();
        }

        public IPicture ProfilePicture => this._picture.PictureView;

        public void Display()
        {
            var signOutElement = new StyledStringElement("Sign Out", this.SignOutButton_Touched)
            {
                Alignment = UITextAlignment.Center,
                TextColor = UIColor.Red,
                BackgroundColor = UIColor.White
            };

            if (_defaultImage == null)
            {
                _defaultImage = UIImage.FromBundle("UserPic");
            }

            this._picture = AccountPictureView.Create(new CGRect(0, 0, this.View.Frame.Width, 90));
            _picture.SetPictureUrl(User.PictureUrl, _defaultImage);
            _picture.BackgroundColor = UIColor.Clear;
            _picture.OnPictureTouched += this.ChangePictureButton_Touched;
            var section = new Section("Profile")
            {
                new StringElement("Full Name", User.FullName),
                new StringElement("Username", User.UserName),
                new StringElement("Email", User.Email),
            };

            section.HeaderView = _picture;

            this.Root = new RootElement("Account")
            {
                section,
                new Section
                {
                    signOutElement
                },
                new Section
                {
                    new StyledStringElement("Terms and conditions", tapped: TermsAndConditionsTapped)
                    {
                        Alignment = UITextAlignment.Center,
                        TextColor = UIColor.DarkGray
                    }
                }
            };
        }

        private void TermsAndConditionsTapped()
        {
            var url = new NSUrl(Constants.TermsAndConditionsLink);
            var sfViewController = new SFSafariViewController(url);
            PresentViewControllerAsync(sfViewController, true);
        }

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);


        partial void EditButton_Touched(UIBarButtonItem sender)
        {
            this._picture.EditButtonHidden = false;
            var readonlyFullName = (StringElement) this.Root[0][0];
            this.Root[0].Remove(readonlyFullName);

            if (_saveButton == null)
            {
                _saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveButton_Touched);
            }

            this.NavigationController.NavigationBar.TopItem.RightBarButtonItem = this._saveButton;
            this.Root[0].Insert(0,
                new EntryElement("Full Name", "First Name and Last Name", readonlyFullName.Value));


            readonlyFullName.Dispose();
        }

        private async void SaveButton_Touched(object sender, EventArgs eventArgs)
        {
            var entryElement = (EntryElement) this.Root[0][0];
            this.User.FullName = entryElement.Value;

            var overlay = new Overlay("Wait");
            overlay.PopUp();
            var success = await this._accountBusinessLogic.SaveAsync();
            if (success)
            {
                this.Root[0].Remove(entryElement);
                this.Root[0].Insert(0, new StringElement("Full Name", entryElement.Value));
                entryElement.Dispose();

                this.NavigationController.NavigationBar.TopItem.RightBarButtonItem = this.EditButton;
                this._picture.EditButtonHidden = true;
            }

            overlay.Close();
        }

        private void SignOutButton_Touched()
        {
            var alert = UIAlertController
                .Create("Sign Out",
                    "Do you really want to sign out?",
                    UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Destructive,
                x => _accountBusinessLogic.SignOut()));
            this.PresentViewController(alert, true, null);
        }

        private void ChangePictureButton_Touched(Picture picture)
        {
            _profilePictureSource.ShowPictureSourceDialog();
        }

        public void ImageSelected(UIImage image)
        {
            this.PictureChanged = true;
            this._picture.SetPicture(image);
        }
    }
}