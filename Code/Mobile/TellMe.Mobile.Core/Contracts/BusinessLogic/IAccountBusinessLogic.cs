using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IAccountBusinessLogic : IBusinessLogic
    {
        IAccountView View { get; set; }
        bool IsAuthenticated { get; }
        bool PushIsEnabled { get; set; }
        Task RegisteredForRemoteNotificationsAsync(string pushToken);
        Task SyncPushTokenAsync();
        void SignOut();
        void NavigateChangePicture();
        void InitView();
        Task<bool> SaveAsync();
    }
}