using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ITribeView : IStoriesListView
    {
        void DisplayTribe(TribeDTO tribe);
        TribeDTO Tribe { get; set; }
        int TribeId { get; }

        void TribeLeft(TribeDTO tribe);
    }
}