using System.Threading.Tasks;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IAccountBusinessLogic : IBusinessLogic
    {
        bool IsAuthenticated { get; }
        bool PushIsEnabled { get; set; }
        Task RegisteredForRemoteNotificationsAsync(string pushToken);
        Task SyncPushTokenAsync();
    }
}