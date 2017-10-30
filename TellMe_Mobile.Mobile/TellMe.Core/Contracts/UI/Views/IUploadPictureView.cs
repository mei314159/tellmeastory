using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IUploadPictureView : IView
    {
        IPicture ProfilePicture { get; }

        void ShowPictureSourceDialog();
    }
}