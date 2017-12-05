using System;
using System.IO;
using System.Timers;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.UI.Views;

namespace TellMe.Mobile.Core.Types.BusinessLogic
{
    public class RecordVideoBusinessLogic : IRecordVideoBusinessLogic
    {
        private readonly IRouter _router;
        private readonly Timer _recordTimer;
        private string _videoPath;
        private DateTime _recordStartTime;
        private volatile bool _deleteVideo;
        private volatile bool _recording;

        public RecordVideoBusinessLogic(IRouter router)
        {
            this._router = router;
            _recordTimer = new Timer(10);
            _recordTimer.Elapsed += RecordTimer_Elapsed;
        }

        public IRecordVideoView View { get; set; }

        public void PreviewStory()
        {
            _router.NavigatePreviewStory(View, this._videoPath, View.StoryRequest, View.RequestNotification,
                View.Contact);
        }

        public void ToggleRecording()
        {
            this.View.RecordButton.Enabled = false;
            if (_recording)
                StopRecording();
            else
                StartRecording();
        }

        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            Console.WriteLine("Deleting File");
            File.Delete(filePath);
            Console.WriteLine("Deleted File");
        }

        public void RecordingStarted(DateTime startDate)
        {
            _recordStartTime = startDate;
            _recordTimer.Start();
            this.View.RecordButton.Enabled = true;
            View.RecordButton.TitleString = "Stop Recording";
        }

        public void RecordingStopped()
        {
            _recordTimer?.Stop();
            Console.WriteLine("stopped recording");
            this.View.RecordButton.Enabled = true;
            this.View.RecordButton.TitleString = "Record";

            if (_deleteVideo)
            {
                DeleteFile(this._videoPath);
                _deleteVideo = false;
            }
            else
                PreviewStory();
        }

        private void StartRecording()
        {
            _recording = true;
            this._videoPath = Path.Combine(Constants.TempVideoStorage, DateTime.Now.Ticks + ".mov");
            DeleteFile(_videoPath);
            View.StartCapture(_videoPath);
            Console.WriteLine(_videoPath);
        }

        private void StopRecording(bool deleteFile = false)
        {
            _recording = false;
            View.StopCapture();
            this._deleteVideo = deleteFile;
        }

        private void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var duration = (DateTime.Now - _recordStartTime);
            View.DurationLabel = duration.ToString("ss\\:ff");
            if (duration.TotalSeconds >= 10)
            {
                View.StopCapture();
            }
        }

        public void WillClose()
        {
            if (_recording)
            {
                StopRecording(true);
            }
        }

        public void SwitchCamera()
        {
            if (this._recording)
            {
                return;
            }

            View.SwitchCamera();
        }
    }
}