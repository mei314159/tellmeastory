using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStorytellersBusinessLogic : IBusinessLogic
    {
        IStorytellersView View { get; set; }
        Task LoadAsync(bool forceRefresh, string searchText);
        Task SendFriendshipRequestAsync(StorytellerDTO storyteller);
        Task SendRequestToJoinPromptAsync(string email);
        void AddTribe();
        void NavigateStoryteller(StorytellerDTO storyteller);
        Task AcceptTribeInvitationAsync(TribeDTO dto);
        Task RejectTribeInvitationAsync(TribeDTO dto);
        void ViewTribe(TribeDTO tribe);
    }
}