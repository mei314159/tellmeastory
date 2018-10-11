using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Contracts.BusinessLogic
{
    public interface IStoryBusinessLogic : IBusinessLogic
    {
        IStoryView View { get; set; }
        void AddToPlaylist(StoryDTO story);
    }
}