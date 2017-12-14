using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Views.Badge;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class StoriesListViewController : StoriesTableViewController, IStoriesListView
    {
        private BadgeView _notificationsBadge;

        public StoriesListViewController(IntPtr handle) : base(handle)
        {
        }

        private new IStoriesBusinessLogic BusinessLogic
        {
            get => (IStoriesBusinessLogic)base.BusinessLogic;
            set => base.BusinessLogic = value;
        }

        public override void ViewDidLoad()
        {
            if (this.BusinessLogic == null)
                this.BusinessLogic = IoC.GetInstance<IStoriesBusinessLogic>();

            base.ViewDidLoad();
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CheckPushNotificationsPermissions();
        }

        public override void ViewWillAppear(bool animated)
        {
            BusinessLogic.LoadActiveNotificationsCountAsync();
        }

        public override void ViewDidAppear(bool animated)
        {
            //_businessLogic.NavigateEvents();
        }

        [Action("UnwindToStoriesViewController:")]
        public void UnwindToStoriesViewController(UIStoryboardSegue segue)
        {
            Task.Run(() => LoadStoriesAsync(true, false));
        }

        partial void RecordButton_Touched(UIBarButtonItem sender)
        {
            BusinessLogic.SendStory();
        }

        partial void RequestButton_Touched(UIBarButtonItem sender)
        {
            BusinessLogic.RequestStory();
        }

        partial void Notifications_Activated(UIBarButtonItem sender)
        {
            BusinessLogic.NotificationsCenter();
        }

        public void DisplayNotificationsCount(int count)
        {
            InvokeOnMainThread(() =>
            {
                if (_notificationsBadge == null)
                {
                    _notificationsBadge =
                        new BadgeView(new CGRect(12, -8, 30, 20))
                        {
                            Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
                            Hidden = true
                        };

                    Notifications.CustomView = new UIImageView(UIImage.FromBundle("Bell"))
                    {
                        Frame = new CGRect(0, 0, 24, 24),
                    };
                    Notifications.CustomView.Add(_notificationsBadge);
                    Notifications.CustomView.AddGestureRecognizer(
                        new UITapGestureRecognizer(() => Notifications_Activated(Notifications)));
                }

                _notificationsBadge.Hidden = count == 0;
                _notificationsBadge.Value = count;
            });
        }
    }
}