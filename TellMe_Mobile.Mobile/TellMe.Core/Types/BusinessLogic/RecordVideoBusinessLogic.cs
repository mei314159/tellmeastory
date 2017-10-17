using System;
using System.IO;
using System.Timers;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.UI.Views;

namespace TellMe.Core.Types.BusinessLogic
{
    public class RecordVideoBusinessLogic
    {
        private IRouter _router;
        private IRecordVideoView _view;
        private string videoPath;
        private volatile bool deleteVideo;
        private volatile bool recording;
        private DateTime recordStartTime;
        private Timer recordTimer;

        public RecordVideoBusinessLogic(IRouter _router, IRecordVideoView _view)
        {
            this._router = _router;
			this._view = _view;
			recordTimer = new Timer(10);
			recordTimer.Elapsed += RecordTimer_Elapsed;
        }

        public void PreviewStory()
        {
            _router.NavigatePreviewStory(_view, this.videoPath, _view.RequestedStory, _view.RequestNotification);
        }

        public void ToggleRecording()
        {
			this._view.RecordButton.Enabled = false;
            if (recording)
                StopRecording();
            else
                StartRecording();
        }

        private void DeleteFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(videoPath) && File.Exists(filePath))
            {
                Console.WriteLine("Deleting File");
                File.Delete(filePath);
                Console.WriteLine("Deleted File");
            }
        }

        public void RecordingStarted(DateTime startDate)
        {
			recordStartTime = startDate;
			recordTimer.Start();
            this._view.RecordButton.Enabled = true;
			_view.RecordButton.TitleString = "Stop Recording";
        }

        public void RecordingStopped()
        {
			recordTimer?.Stop();
            Console.WriteLine("stopped recording");
            this._view.RecordButton.Enabled = true;
            this._view.RecordButton.TitleString = "Record";

            if (deleteVideo)
            {
                DeleteFile(this.videoPath);
                deleteVideo = false;
            }
            else
                PreviewStory();
        }

        private void StartRecording()
		{
			recording = true;
            this.videoPath = Path.Combine(Constants.TempVideoStorage, DateTime.Now.Ticks + ".mov");
            DeleteFile(videoPath);
            _view.StartCapture(videoPath);
            Console.WriteLine(videoPath);
        }

        private void StopRecording(bool deleteFile = false)
		{
            recording = false;
            _view.StopCapture();
            this.deleteVideo = deleteFile;
        }

        void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
			TimeSpan duration = (DateTime.Now - recordStartTime);
			_view.DurationLabel = duration.ToString("ss\\:ff");
			if (duration.TotalSeconds >= 10)
			{
                _view.StopCapture();
			}
		}

        public void WillClose()
        {
            if (recording)
            {
                StopRecording(true);
            }
        }

        public void SwitchCamera()
        {
            if (this.recording)
            {
                return;
            }

            _view.SwitchCamera();
        }
    }
}
