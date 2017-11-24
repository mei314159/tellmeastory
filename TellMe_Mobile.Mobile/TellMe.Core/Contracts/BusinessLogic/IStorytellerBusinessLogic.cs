using System.Threading.Tasks;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IStorytellerBusinessLogic : IStoriesTableBusinessLogic
    {
        new IStorytellerView View { get; set; }
        void SendStory();
        void RequestStory();
    }
}