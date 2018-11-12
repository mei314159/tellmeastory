using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using SDWebImage;
using TellMe.iOS.Core;
using TellMe.iOS.Core.DTO;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;
using UserNotifications;

namespace TellMe.iOS
{
    public static class ShortcutIdentifier
    {
        public const string RecordStory = "com.mehspot.tellmeastory.recordstory";
    }

    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        // class-level declarations

        private IAccountBusinessLogic AccountBusinessLogic { get; set; }

        public UIApplicationShortcutItem LaunchedShortcutItem { get; set; }

        public override UIWindow Window { get; set; }

        public bool HandleShortcutItem(UIApplicationShortcutItem shortcutItem)
        {
            var handled = false;

            // Anything to process?
            if (shortcutItem == null) 
                return false;

            var tabbarController = (UITabBarController)this.Window.RootViewController;

            // Take action based on the shortcut type
            if (shortcutItem.Type == ShortcutIdentifier.RecordStory)
            {
                IoC.GetInstance<IRouter>().NavigateRecordStory(tabbarController.AsIView());
                handled = true;
            }

            // Return results
            return handled;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            InitializeAppCenter();
            InitializeSDWebImage();
            var window = new UIWindow(UIScreen.MainScreen.Bounds);
            IoC.Initialize(window);

            this.AccountBusinessLogic = IoC.GetInstance<IAccountBusinessLogic>();
            App.Instance.Initialize();
            App.Instance.OnNotificationReceived += Instance_OnNotificationReceived;

            this.Window = window;
            this.Window.RootViewController = GetInitialViewController();
            this.Window.MakeKeyAndVisible();

            var notification =
                (NSDictionary) launchOptions?.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey);
            if (notification != null)
            {
                ProcessNotification(notification);
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            }

            UNUserNotificationCenter.Current.Delegate = this;

            var shouldPerformAdditionalDelegateHandling = true;

            // Get possible shortcut item
            if (launchOptions != null)
            {
                LaunchedShortcutItem = launchOptions[UIApplication.LaunchOptionsShortcutItemKey] as UIApplicationShortcutItem;
                shouldPerformAdditionalDelegateHandling = (LaunchedShortcutItem == null);
            }

            return shouldPerformAdditionalDelegateHandling;
        }

        private void InitializeSDWebImage()
        {
            SDWebImageManager.SharedManager.ImageDownloader.MaxConcurrentDownloads = 3;
            SDWebImageManager.SharedManager.ImageCache.Config.ShouldCacheImagesInMemory = false;
            SDImageCache.SharedImageCache.Config.ShouldCacheImagesInMemory = false;
        }

        private void InitializeAppCenter()
        {
            AppCenter.Start("ac8e4d56-035e-41b2-b764-61ea68a34745",
                   typeof(Analytics), typeof(Crashes));
            AppCenter.Start("ac8e4d56-035e-41b2-b764-61ea68a34745", typeof(Analytics), typeof(Crashes));
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

        public override void OnActivated(UIApplication application)
        {
            //if (this.AccountBusinessLogic.IsAuthenticated)
            //{
            //    await SyncContactsBusinessLogic.SynchronizeContacts();
            //}

            // Handle any shortcut item being selected
            HandleShortcutItem(LaunchedShortcutItem);

            // Clear shortcut after it's been handled
            LaunchedShortcutItem = null;
        }

        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            // Perform action
            completionHandler(HandleShortcutItem(shortcutItem));
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
        public virtual void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge |
                              UNNotificationPresentationOptions.Sound);
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo);
        }

        public void CheckPushNotificationsPermissions()
        {
            if (this.AccountBusinessLogic.PushIsEnabled)
            {
                RegisterPushNotifications();
            }
            else
            {
                var alert = UIAlertController.Create(
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
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge,
                (granted, error) =>
                {
                    if (granted)
                    {
                        this.InvokeOnMainThread(() => UIApplication.SharedApplication.RegisterForRemoteNotifications());
                    }
                });
        }

        private void ProcessNotification(NSDictionary userInfo)
        {
            var pushJson = new NSString(NSJsonSerialization.Serialize(userInfo, 0, out _), NSStringEncoding.UTF8)
                .ToString();
            var notification = JsonConvert.DeserializeObject<PushNotification>(pushJson);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = notification.Data.Badge ?? 1;
            App.Instance.NotificationReceived(new NotificationDTO
            {
                Id = notification.NotificationId ?? default(int),
                Text = notification.Data.Message,
                Extra = notification.Extra,
                Type = notification.NotificationType
            });
        }

        private async void Instance_OnNotificationReceived(NotificationDTO notification)
        {
            if (!(Window.RootViewController is UINavigationController navigationController))
                return;

            var view = navigationController.ChildViewControllers.OfType<IView>().FirstOrDefault();
                await IoC.GetInstance<INotificationHandler>().ProcessNotificationAsync(notification, view)
                    .ConfigureAwait(false);
        }

        private UIViewController GetInitialViewController()
        {
            if (!this.AccountBusinessLogic.IsAuthenticated)
            {
                return UIStoryboard.FromName("Auth", null).InstantiateInitialViewController();
            }

            return UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }
    }
}