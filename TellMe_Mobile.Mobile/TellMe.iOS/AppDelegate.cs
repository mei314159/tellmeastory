using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDWebImage;
using TellMe.Core;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.DTO;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Types.DataServices.Local;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Core;
using TellMe.iOS.Core.DTO;
using TellMe.iOS.Core.Providers;
using TellMe.iOS.Extensions;
using UIKit;
using UserNotifications;

namespace TellMe.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public AccountBusinessLogic AccountBusinessLogic { get; private set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            SDWebImageManager.SharedManager.ImageDownloader.MaxConcurrentDownloads = 3;
            SDWebImageManager.SharedManager.ImageCache.ShouldCacheImagesInMemory = false;
            SDImageCache.SharedImageCache.ShouldCacheImagesInMemory = false;

            UIWindow window = new UIWindow(UIScreen.MainScreen.Bounds);
            var accountService = new AccountService();
            var applicationDataStorage = new ApplicationDataStorage();
            var remotePushDataService = new RemotePushDataService();
            var contactsProvider = new ContactsProvider();
            this.AccountBusinessLogic = new AccountBusinessLogic(applicationDataStorage, accountService, remotePushDataService);
            App.Instance.Initialize(accountService, applicationDataStorage, new Router(window));

            this.Window = window;
            this.Window.RootViewController = GetInitialViewController(launchOptions);
            this.Window.MakeKeyAndVisible();

            if (launchOptions != null)
            {
                var notification = (NSDictionary)launchOptions.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey);
                if (notification != null)
                {
                    ProcessNotification(notification, false);
                }
            }

            UNUserNotificationCenter.Current.Delegate = this;

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override async void OnActivated(UIApplication application)
        {
            if (this.AccountBusinessLogic.IsAuthenticated)
            {
                //await SyncContactsBusinessLogic.SynchronizeContacts();
            }
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Get current device token
            var newDeviceToken = deviceToken.Description?.Trim('<').Trim('>').Replace(" ", string.Empty);
            if (!string.IsNullOrWhiteSpace(newDeviceToken))
            {
                Task.Run(() => this.AccountBusinessLogic.RegisteredForRemoteNotificationsAsync(newDeviceToken));
            }
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public virtual void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge | UNNotificationPresentationOptions.Sound);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo, application.ApplicationState == UIApplicationState.Active);
        }

        public void CheckPushNotificationsPermissions()
        {
            if (this.AccountBusinessLogic.PushIsEnabled)
            {
                RegisterPushNotifications();
            }
            else
            {
                UIAlertController alert = UIAlertController.Create(
                                            "Enable push notifications",
                    "Do you want to get notified about unread messages?",
                    UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                alert.AddAction(UIAlertAction.Create("Yes, I do", UIAlertActionStyle.Default, (e) =>
                {
                    this.AccountBusinessLogic.PushIsEnabled = true;

                    RegisterPushNotifications();

                }));

                UIApplication
                    .SharedApplication
                    .KeyWindow
                    .RootViewController
                    .PresentViewController(alert, true, null);
            }
        }

        private void RegisterPushNotifications()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge, (granted, error) =>
            {
                if (granted)
                {
                    this.InvokeOnMainThread(() => UIApplication.SharedApplication.RegisterForRemoteNotifications());
                }
            });
        }

        private void ProcessNotification(NSDictionary userInfo, bool quiet = true)
        {
            NSError error = new NSError();
            var pushJson = new NSString(NSJsonSerialization.Serialize(userInfo, 0, out error), NSStringEncoding.UTF8).ToString();
            var notification = JsonConvert.DeserializeObject<PushNotification>(pushJson);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = notification.Data.Badge ?? 1;
            var tabBarController = Window.RootViewController as UITabBarController;
            if (tabBarController == null)
                return;

            foreach (var c in tabBarController.ViewControllers)
            {
                var rootController = c as UINavigationController;
                if (rootController == null)
                {
                    continue;
                }

                if (notification.NotificationType == NotificationTypeEnum.StoryRequest)
                {
                    var controller = rootController.ChildViewControllers.OfType<IView>().FirstOrDefault();
                    App.Instance.Router.NavigateRecordStory(controller, requestedStory: ((JObject)notification.Extra).ToObject<StoryDTO>());
                }
                else if (notification.NotificationType == NotificationTypeEnum.Story)
                {
                }
                else if (notification.NotificationType == NotificationTypeEnum.FriendshipRequest)
                {
                }
                else if (notification.NotificationType == NotificationTypeEnum.FriendshipAccepted)
                {
                }
                else if (notification.NotificationType == NotificationTypeEnum.FriendshipRejected)
                {
                }
            }
        }

        private UIViewController GetInitialViewController(NSDictionary launchOptions)
        {
            if (!this.AccountBusinessLogic.IsAuthenticated)
            {
                return UIStoryboard.FromName("Auth", null).InstantiateInitialViewController();
            }

            return UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }
}

