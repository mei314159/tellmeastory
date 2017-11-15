using System.Threading.Tasks;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IUploadPictureBusinessLogic : IBusinessLogic
    {
        IUploadPictureView View { get; set; }
        void Init();
        void SkipButtonTouched();
        void SelectPictureTouched();
        Task PictureSelectedAsync();
    }
}