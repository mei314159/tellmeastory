using System;
using Foundation;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using TellMe.iOS.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class SendStoryViewController : UIViewController, ISendStoryView
    {
        private ISendStoryBusinessLogic _businessLogic;

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
            _businessLogic = IoC.GetInstance<ISendStoryBusinessLogic>();
            _businessLogic.View = this;
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            this._businessLogic.Init();

            var text = new NSMutableAttributedString();
            text.Append(new NSAttributedString(_businessLogic.GetUsername(),
                UIFont.BoldSystemFontOfSize(StoryLabel.Font.PointSize)));
            text.Append(new NSAttributedString($" has shared with you a story about:"));
            StoryLabel.AttributedText = text;
            this.StoryTitle.EditingChanged += (sender, e) => this._businessLogic.InitButtons();
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

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