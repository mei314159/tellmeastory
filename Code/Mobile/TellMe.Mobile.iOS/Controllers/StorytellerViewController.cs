using System;
using Foundation;
using CoreGraphics;
using TellMe.iOS.Core;
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
            this.NavigationController?.SetNavigationBarHidden(true, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            this.NavigationController?.SetNavigationBarHidden(false, false);
        }

        public void DisplayStoryteller(StorytellerDTO storyteller)
        {
            InvokeOnMainThread(() =>
            {
                this.UserName.Text = storyteller.UserName;
                this.FullName.Text = storyteller.FullName;
                this.FriendsCount.Text = storyteller.FriendsCount.ToString();
                this.EventsCount.Text = storyteller.EventsCount.ToString();
                this.StoriesCount.Text = storyteller.StoriesCount.ToString();
                this.ProfilePicture.SetPictureUrl(storyteller.PictureUrl, UIImage.FromBundle("UserPic"));
                
                this.HeaderView.Layer.MasksToBounds = false;
                this.HeaderView.Layer.ShadowOffset = new CGSize(0, 2);
                this.HeaderView.Layer.ShadowRadius = 1;
                this.HeaderView.Layer.ShadowOpacity = 0.5f;
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

        partial void BackButtonTouched(UIButton sender, UIEvent @event)
        {
            NavigationController.PopViewController(true);
        }
    }
}