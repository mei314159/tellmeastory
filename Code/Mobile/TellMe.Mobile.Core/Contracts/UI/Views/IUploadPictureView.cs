using TellMe.Mobile.Core.Contracts.UI.Components;

namespace TellMe.Mobile.Core.Contracts.UI.Views
{
    public interface IUploadPictureView : IView
    {
        IPicture ProfilePicture { get; }

        void ShowPictureSourceDialog();
    }
}