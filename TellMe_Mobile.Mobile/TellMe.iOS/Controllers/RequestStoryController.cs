using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core;
using TellMe.Core.Types.BusinessLogic;
using TellMe.iOS.Extensions;
using TellMe.Core.Types.DataServices.Remote;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Contracts.UI.Components;
using TellMe.iOS.Views;
using System.Linq;

namespace TellMe.iOS
{
    public partial class RequestStoryController : UIViewController, IRequestStoryView
    {
        private RequestStoryBusinessLogic _businessLogic;
        public RequestStoryController(IntPtr handle) : base(handle)
        {
        }

        public ICollection<ContactDTO> Recipients { get; set; }

        ITextInput IRequestStoryView.StoryTitle => this.StoryTitle;

        IButton IRequestStoryView.SendButton => this.SendButton;

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) => ViewExtensions.ShowSuccessMessage(this, message, complete);

        public override void ViewDidLoad()
        {
            StoryTitle.EditingChanged += StoryTitle_EditingChanged;
            _businessLogic = new RequestStoryBusinessLogic(this, App.Instance.Router, new RemoteStoriesDataService());
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            UpdatePreview();
        }

        async partial void SendButton_TouchUpInside(Button sender)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.SendAsync();
            overlay.Close();
        }

        void StoryTitle_EditingChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var text = new NSMutableAttributedString();
            string handleName = null;
            if (Recipients?.Count == 1)
            {
                var contactDTO = Recipients.First();
                if (contactDTO.Type == ContactType.User)
                {
                    handleName = contactDTO.User.UserName;
                }
            }

            if (handleName == null)
            {
                handleName = "{Handle}";
            }

            text.Append(new NSAttributedString(handleName, UIFont.BoldSystemFontOfSize(RequestTextPreview.Font.PointSize), UIColor.Blue));
            text.Append(new NSAttributedString(","));
            text.Append(new NSAttributedString(App.Instance.AuthInfo.Account.UserName, UIFont.BoldSystemFontOfSize(RequestTextPreview.Font.PointSize)));
            text.Append(new NSAttributedString($" would like you to tell a story about: "));
            text.Append(new NSAttributedString(StoryTitle.Text, UIFont.ItalicSystemFontOfSize(RequestTextPreview.Font.PointSize)));
            RequestTextPreview.AttributedText = text;
        }

        public void Close(ICollection<StoryRequestDTO> requestedStory)
        {
            PerformSegue("UnwindToStories", this);
        }
    }
}