using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class RequestStoryController : UIViewController, IRequestStoryView
    {
        private IRequestStoryBusinessLogic _businessLogic;

        public RequestStoryController(IntPtr handle) : base(handle)
        {
        }

        public event StoryRequestCreatedEventHandler RequestCreated;
        
        public ICollection<ContactDTO> Recipients { get; set; }

        ITextInput IRequestStoryView.StoryTitle => this.StoryTitle;

        IButton IRequestStoryView.SendButton => this.SendButton;

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void ShowSuccessMessage(string message, Action complete = null) =>
            ViewExtensions.ShowSuccessMessage(this, message, complete);

        public override void ViewDidLoad()
        {
            StoryTitle.EditingChanged += StoryTitle_EditingChanged;
            _businessLogic = IoC.GetInstance<IRequestStoryBusinessLogic>();
            _businessLogic.View = this;
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            UpdatePreview();
        }

        async partial void SendButton_TouchUpInside(Button sender)
        {
            var overlay = new Overlay("Wait");
            overlay.PopUp(true);
            await _businessLogic.CreateStoryRequest();
            overlay.Close();
        }

        private void StoryTitle_EditingChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            var text = new NSMutableAttributedString();
            string handleName = null;
            if (Recipients?.Count == 1)
            {
                var contactDto = Recipients.First();
                if (contactDto.Type == ContactType.User)
                {
                    handleName = contactDto.User.UserName;
                }
            }

            if (handleName == null)
            {
                handleName = "{Handle}";
            }
            text.Append(new NSAttributedString(handleName,
                UIFont.BoldSystemFontOfSize(RequestTextPreview.Font.PointSize), UIColor.Blue));
            text.Append(new NSAttributedString(","));
            text.Append(new NSAttributedString(_businessLogic.GetUsername(),
                UIFont.BoldSystemFontOfSize(RequestTextPreview.Font.PointSize)));
            text.Append(new NSAttributedString($" would like you to tell a story about: "));
            text.Append(new NSAttributedString(StoryTitle.Text,
                UIFont.ItalicSystemFontOfSize(RequestTextPreview.Font.PointSize)));
            RequestTextPreview.AttributedText = text;
        }

        public void Close(RequestStoryDTO dto, ICollection<ContactDTO> recipients)
        {
            RequestCreated?.Invoke(dto, recipients);
            if (NavigationController != null)
                this.NavigationController.PopViewController(true);
            else
                this.DismissViewController(true, null);
        }
    }
}