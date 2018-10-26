using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface INotificationHandlerBusinessLogic : IBusinessLogic
    {
        Task<bool> RejectFriendshipRequestAsync(int notificationId, StorytellerDTO dto);
        Task<bool> AcceptFriendshipRequestAsync(int notificationId, StorytellerDTO dto);
        Task<bool> RejectTribeInvitationAsync(int notificationId, TribeDTO dto);
        Task<bool> AcceptTribeInvitationAsync(int notificationId, TribeDTO dto);
        Task<bool> NavigateStory(int notificationId, StoryDTO story, IView view);
        Task<bool> NavigatePlaylist(int notificationId, PlaylistDTO playlist, IView view);
        Task<bool> NavigateEvent(int notificationId, EventDTO eventDTO, IView view);
        Task<bool> RejectStoryRequestRequestAsync(int notificationId, StoryRequestDTO dto);
        Task<bool> AcceptStoryRequestRequest(int notificationId, StoryRequestDTO dto, IView view);
        Task<bool> HandleNotification(int notificationId);
    }
}