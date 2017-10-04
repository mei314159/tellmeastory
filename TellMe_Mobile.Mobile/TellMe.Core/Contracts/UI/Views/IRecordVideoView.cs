using TellMe.Core.Contracts.UI.Components;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface IRecordVideoView : IView
    {
        IButton RecordButton { get; }
        string DurationLabel { get; set; }
        void StartCapture(string videoPath);
        void StopCapture();
        void Close();
        void SwitchCamera();
    }
}