using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.Handlers;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using TellMe.iOS.Views.Cells;
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
            this.NavigationController.SetToolbarHidden(true, false);
        }

        public void DisplayTribe(TribeDTO tribe)
        {
            InvokeOnMainThread(() =>
            {
                //NavItem.Title = tribe.Name;
                this.TribeName.Text = tribe.Name;
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

        void ITribeView.TribeLeft(TribeDTO tribe)
        {
            TribeLeft?.Invoke(tribe);
        }
    }
}