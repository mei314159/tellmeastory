using System;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Views;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class StorytellerViewController : StoriesTableViewController, IStorytellerView
    {
        public StorytellerDTO Storyteller { get; set; }
        public string StorytellerId { get; set; }

        public StorytellerViewController(IntPtr handle) : base(handle)
        {
        }

        private new IStorytellerBusinessLogic BusinessLogic
        {
            get => (IStorytellerBusinessLogic)base.BusinessLogic;
            set => base.BusinessLogic = value;
        }

        public override void ViewDidLoad()
        {
            if (this.BusinessLogic == null)
                this.BusinessLogic = IoC.GetInstance<IStorytellerBusinessLogic>();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.NavigationController.SetToolbarHidden(true, false);
        }

        public void DisplayStoryteller(StorytellerDTO storyteller)
        {
            InvokeOnMainThread(() =>
            {
                NavItem.Title = storyteller.UserName;
                this.UserName.Text = storyteller.UserName;
                this.FullName.Text = storyteller.FullName;
                this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl, UIImage.FromBundle("UserPic"));
            });
        }

        private void SendStoryButtonTouched(object sender, EventArgs e)
        {
            BusinessLogic.SendStory();
        }

        private void RequestStoryButtonTouched(object sender, EventArgs e)
        {
            BusinessLogic.RequestStory();
        }

        partial void RequestStoryTouched(NSObject sender)
        {
            BusinessLogic.RequestStory();
        }

        partial void SendStoryTouched(NSObject sender)
        {
            BusinessLogic.SendStory();
        }
    }
}