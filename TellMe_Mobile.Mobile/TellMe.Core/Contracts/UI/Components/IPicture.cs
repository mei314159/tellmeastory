using System.IO;

namespace TellMe.Core.Contracts.UI.Components
{
    public interface IPicture
    {
        Stream GetPictureStream();
        void SetPictureUrl(string pictureUrl, object defaultPicture);
    }
}
