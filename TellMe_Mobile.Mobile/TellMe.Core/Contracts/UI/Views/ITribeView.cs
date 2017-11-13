using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ITribeView : IView
    {
        void ShowSuccessMessage(string message, Action complete = null);

        void DisplayStories(ICollection<StoryDTO> stories);
        void DisplayTribe(TribeDTO tribe);
        TribeDTO Tribe { get; set; }
        int TribeId { get; }

        void TribeLeft(TribeDTO tribe);
    }
}