using System;
using AVFoundation;
using CoreMedia;
using Foundation;
using TellMe.iOS.Core;
using TellMe.iOS.Core.UI;
using TellMe.iOS.Extensions;
using TellMe.Mobile.Core.Contracts.BusinessLogic;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Contracts.UI.Components;
using TellMe.Mobile.Core.Contracts.UI.Views;
using UIKit;

namespace TellMe.iOS.Controllers
{
    public partial class RecordVideoController : UIViewController, IRecordVideoView
    {
        private AVCaptureMovieFileOutput _output;
        private AVCaptureDevice _device;
        private AVCaptureDevice _audioDevice;

        private AVCaptureDeviceInput _input;
        private AVCaptureDeviceInput _audioInput;
        private AVCaptureSession _session;
        private AVCaptureVideoPreviewLayer _previewlayer;
        private OutputDelegate _outputDelegate;
        private IRecordVideoBusinessLogic _businessLogic;

        public RecordVideoController(IntPtr handle) : base(handle)
        {
        }

        string IRecordVideoView.DurationLabel
        {
            get => this.Duration.Text;
            set { InvokeOnMainThread(() => Duration.Text = value); }
        }

        IButton IRecordVideoView.RecordButton => this.RecordButton;

        public StoryRequestDTO StoryRequest { get; set; }
        public NotificationDTO RequestNotification { get; set; }
        public ContactDTO Contact { get; set; }
        public EventDTO Event { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _businessLogic = IoC.GetInstance<IRecordVideoBusinessLogic>();
            _businessLogic.View = this;
            _session = new AVCaptureSession();
            Console.WriteLine("getting device inputs");
            try
            {
                //add video capture device
                _device = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
                _input = AVCaptureDeviceInput.FromDevice(_device);
                _session.AddInput(_input);

                //add audio capture device
                _audioDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Audio);
                _audioInput = AVCaptureDeviceInput.FromDevice(_audioDevice);
                _session.AddInput(_audioInput);
                _outputDelegate = new OutputDelegate();
                _outputDelegate.Started += _businessLogic.RecordingStarted;
                _outputDelegate.Stopped += _businessLogic.RecordingStopped;
            }
            catch (Exception ex)
            {
                return;
            }

            Console.WriteLine("setting up preview layer");
            _previewlayer = new AVCaptureVideoPreviewLayer(_session);
            _previewlayer.Frame = this.PreviewView.Bounds;
            PreviewView.Layer.AddSublayer(_previewlayer);

            Console.WriteLine("Configuring output");
            _output = new AVCaptureMovieFileOutput();
            long totalSeconds = 180000;
            var preferredTimeScale = 30;
            CMTime maxDuration = new CMTime(totalSeconds, preferredTimeScale);
            _output.MinFreeDiskSpaceLimit = 1024 * 1024;
            _output.MaxRecordedDuration = maxDuration;

            if (_session.CanAddOutput(_output))
            {
                _session.AddOutput(_output);
            }

            _session.SessionPreset = AVCaptureSession.Preset640x480;
            Console.WriteLine("About to start running session");
        }

        public override void ViewWillAppear(bool animated)
        {
            this.Duration.Text = "00:00";
            _session.StartRunning();
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            _businessLogic.WillClose();
            _session.StopRunning();
            base.ViewWillDisappear(animated);
        }

        public void ShowErrorMessage(string title, string message = null) =>
            ViewExtensions.ShowErrorMessage(this, title, message);

        public void StartCapture(string videoPath)
        {
            _output.StartRecordingToOutputFile(new NSUrl(videoPath, false), _outputDelegate);
        }

        public void StopCapture()
        {
            _output.StopRecording();
        }

        public void SwitchCamera()
        {
            _session.StopRunning();
            var currentPosition = _device.Position;
            this._session.RemoveInput(_input);
            _device.Dispose();
            _input.Dispose();

            var newPosition = currentPosition == AVCaptureDevicePosition.Front
                ? AVCaptureDevicePosition.Back
                : AVCaptureDevicePosition.Front;
            _device = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video,
                newPosition);
            _input = AVCaptureDeviceInput.FromDevice(_device);
            this._session.AddInput(_input);
            _session.StartRunning();
        }

        partial void RecordButtonTouched(Button sender)
        {
            _businessLogic.ToggleRecording();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _output?.Dispose();
                _session?.Dispose();
                _input?.Dispose();
                _audioInput?.Dispose();
                _device?.Dispose();
                _audioDevice?.Dispose();
            }
            Console.WriteLine(String.Format("{0} controller disposed - {1}", this.GetType(), this.GetHashCode()));
            base.Dispose(disposing);
        }

        partial void SwitchCameraButtonTouched(UIBarButtonItem sender)
        {
            _businessLogic.SwitchCamera();
        }

        partial void CloseButtonTouched(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }
    }
}