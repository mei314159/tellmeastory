using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.Handlers;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class TribeViewController : StoriesTableViewController, ITribeView
    {
        private new ITribeViewBusinessLogic BusinessLogic
        {
            get => (ITribeViewBusinessLogic) base.BusinessLogic;
            set => base.BusinessLogic = value;
        }

        public TribeViewController(IntPtr handle) : base(handle)
        {
        }

        public event TribeLeftHandler TribeLeft;
        public TribeDTO Tribe { get; set; }
        public int TribeId { get; set; }

        
        public override void ViewDidLoad()
        {
            if (this.BusinessLogic == null)
                this.BusinessLogic = IoC.GetInstance<ITribeViewBusinessLogic>();

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

        public void DisplayTribe(TribeDTO tribe)
        {
            InvokeOnMainThread(() =>
            {
                this.TribeName.Text = tribe.Name;
                this.MembersCount.Text = tribe.MembersCount.ToString();
                this.EventsCount.Text = tribe.EventsCount.ToString();
                this.StoriesCount.Text = tribe.StoriesCount.ToString();
                this.HeaderView.Layer.MasksToBounds = false;
                this.HeaderView.Layer.ShadowOffset = new CGSize(0, 2);
                this.HeaderView.Layer.ShadowRadius = 1;
                this.HeaderView.Layer.ShadowOpacity = 0.5f;
            });
        }

        partial void SendStoryTouched(NSObject sender)
        {
            BusinessLogic.SendStory();
        }

        partial void InfoButtonTouched(NSObject sender)
        {
            BusinessLogic.TribeInfo();
        }

        partial void RequestStoryTouched(NSObject sender)
        {
            BusinessLogic.RequestStory();
        }

        partial void BackButtonTouched(NSObject sender)
        {
            NavigationController.PopViewController(true);
        }

        void ITribeView.TribeLeft(TribeDTO tribe)
        {
            TribeLeft?.Invoke(tribe);
        }
    }
}