using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IStorytellersView : IView
    {
        void ShowSuccessMessage(string message);

        void DisplayStorytellers(ICollection<StorytellerDTO> items);
    }


}