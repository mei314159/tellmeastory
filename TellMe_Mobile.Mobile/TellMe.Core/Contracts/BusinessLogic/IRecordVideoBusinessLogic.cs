using System;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Contracts.BusinessLogic
{
    public interface IRecordVideoBusinessLogic : IBusinessLogic
    {
        IRecordVideoView View { get; set; }
        void PreviewStory();
        void ToggleRecording();
        void RecordingStarted(DateTime startDate);
        void RecordingStopped();
        void WillClose();
        void SwitchCamera();
    }
}