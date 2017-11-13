using Foundation;
using System;
using UIKit;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;

namespace TellMe.iOS
{
    public partial class SendStoryViewController : UIViewController, ISendStoryView
    {
        SendStoryBusinessLogic _businessLogic;

        public SendStoryViewController(IntPtr handle) : base(handle)
        {
        }

        public string VideoPath { get; set; }

        ITextInput ISendStoryView.StoryTitle => this.StoryTitle;
        IButton ISendStoryView.SendButton => this.SendButton;
        IButton ISendStoryView.ChooseRecipientsButton => this.ChooseRecipientsButton;

		public StoryRequestDTO StoryRequest { get; set; }
        public string PreviewImagePath { get; set; }

        public NotificationDTO RequestNotification { get; set; }
        public ContactDTO Contact { get; set; }

        public override void ViewDidLoad()
        {
            _businessLogic = new SendStoryBusinessLogic(this, App.Instance.Router, new RemoteStoriesDataService());
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            this._businessLogic.Init();

            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString(App.Instance.AuthInfo.Account.UserName, UIFont.BoldSystemFontOfSize(StoryLabel.Font.PointSize)));
            text.Append(new NSAttributedString($" has shared with you a story about:"));
            StoryLabel.AttributedText = text;
            this.StoryTitle.EditingChanged += (sender, e) => this._businessLogic.InitButtons();
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        partial void ChooseRecipientsButton_TouchUpInside(Button sender)
        {
            _businessLogic.ChooseRecipients();
        }

        async partial void SendButton_TouchUpInside(Button sender)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.SendAsync();
            overlay.Close();
        }

        public void Close()
        {
            PerformSegue("UnwindToStories", this);
        }
    }
}