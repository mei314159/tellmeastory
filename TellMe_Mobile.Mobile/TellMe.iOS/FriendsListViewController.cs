using Foundation;
using System;
using UIKit;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class FriendsListViewController : UIViewController
    {
        public FriendsListViewController (IntPtr handle) : base (handle)
        {
        }

        partial void UIButton559_TouchUpInside(UIButton sender)
        {
            this.View.Window.SwapController(UIStoryboard.FromName("Auth", null).InstantiateViewController("ImportContactsController"));
        }
    }
}