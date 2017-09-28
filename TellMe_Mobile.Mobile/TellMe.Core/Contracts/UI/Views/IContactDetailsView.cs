using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IContactDetailsView : IView
    {
        ContactDTO ContactDTO { get; set; }

        void DisplayContactDetails(ContactDTO dto);
        void DisplayStoryDetailsPrompt();
        void ShowSuccessMessage(string message);
        void DisplayStories(ICollection<StoryDTO> stories);
    }
}