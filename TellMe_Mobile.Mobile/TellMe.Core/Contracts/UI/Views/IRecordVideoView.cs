using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IRecordVideoView : IView
	{
        StoryRequestDTO StoryRequest { get; }
        NotificationDTO RequestNotification { get; }
        ContactDTO Contact { get; set; }
        IButton RecordButton { get; }
        string DurationLabel { get; set; }
        void StartCapture(string videoPath);
        void StopCapture();
        void SwitchCamera();
    }
}