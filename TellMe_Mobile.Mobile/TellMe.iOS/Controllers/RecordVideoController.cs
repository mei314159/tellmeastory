using Foundation;
using System;
using UIKit;
using AVFoundation;
using TellMe.Core;
using CoreMedia;
using TellMe.Core.Contracts.UI.Views;
using TellMe.Core.Types.BusinessLogic;
using TellMe.Core.Contracts.UI.Components;
using TellMe.Core.Contracts.DTO;
using TellMe.iOS.Extensions;

namespace TellMe.iOS
{
    public partial class RecordVideoController : UIViewController, IRecordVideoView
    {
        AVCaptureMovieFileOutput output;
        AVCaptureDevice device;
        AVCaptureDevice audioDevice;

        AVCaptureDeviceInput input;
        AVCaptureDeviceInput audioInput;
        AVCaptureSession session;
        AVCaptureVideoPreviewLayer previewlayer;
        OutputDelegate outputDelegate;
        private RecordVideoBusinessLogic businessLogic;

        public RecordVideoController(IntPtr handle) : base(handle)
        {
        }

        string IRecordVideoView.DurationLabel
        {
            get => this.Duration.Text;
            set
            {
                InvokeOnMainThread(() => Duration.Text = value);
            }
        }

        IButton IRecordVideoView.RecordButton => this.RecordButton;

        public StoryRequestDTO StoryRequest { get; set; }
        public NotificationDTO RequestNotification { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            businessLogic = new RecordVideoBusinessLogic(App.Instance.Router, this);

            session = new AVCaptureSession();
            Console.WriteLine("getting device inputs");
            try
            {
                //add video capture device
                device = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
                input = AVCaptureDeviceInput.FromDevice(device);
                session.AddInput(input);

                //add audio capture device
                audioDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Audio);
                audioInput = AVCaptureDeviceInput.FromDevice(audioDevice);
                session.AddInput(audioInput);
                outputDelegate = new OutputDelegate();
                outputDelegate.Started += businessLogic.RecordingStarted;
                outputDelegate.Stopped += businessLogic.RecordingStopped;
            }
            catch (Exception ex)
            {
                return;
            }

            Console.WriteLine("setting up preview layer");
            previewlayer = new AVCaptureVideoPreviewLayer(session);
            previewlayer.Frame = this.PreviewView.Bounds;
            PreviewView.Layer.AddSublayer(previewlayer);

            Console.WriteLine("Configuring output");
            output = new AVCaptureMovieFileOutput();

            long totalSeconds = 10000;
            var preferredTimeScale = 30;
            CMTime maxDuration = new CMTime(totalSeconds, preferredTimeScale);
            output.MinFreeDiskSpaceLimit = 1024 * 1024;
            output.MaxRecordedDuration = maxDuration;

            if (session.CanAddOutput(output))
            {
                session.AddOutput(output);
            }

            session.SessionPreset = AVCaptureSession.Preset640x480;
            Console.WriteLine("About to start running session");
        }

        public override void ViewWillAppear(bool animated)
        {
            this.Duration.Text = "00:00";
            session.StartRunning();
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            businessLogic.WillClose();
            session.StopRunning();
            base.ViewWillDisappear(animated);
        }

        public void ShowErrorMessage(string title, string message = null) => ViewExtensions.ShowErrorMessage(this, title, message);

        public void StartCapture(string videoPath)
        {
            output.StartRecordingToOutputFile(new NSUrl(videoPath, false), outputDelegate);
        }

        public void StopCapture()
        {
            output.StopRecording();
        }

        public void SwitchCamera()
        {
            var type = device.DeviceType;
            session.StopRunning();
            var currentPosition = device.Position;
            this.session.RemoveInput(input);
            device.Dispose();
            input.Dispose();

            var newPosition = currentPosition == AVCaptureDevicePosition.Front ? AVCaptureDevicePosition.Back : AVCaptureDevicePosition.Front;
            device = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, newPosition);
            input = AVCaptureDeviceInput.FromDevice(device);
            this.session.AddInput(input);
            session.StartRunning();
        }

        partial void RecordButtonTouched(Button sender)
        {
            businessLogic.ToggleRecording();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                output?.Dispose();
                session?.Dispose();
                input?.Dispose();
                audioInput?.Dispose();
                device?.Dispose();
                audioDevice?.Dispose();
            }
            Console.WriteLine(String.Format("{0} controller disposed - {1}", this.GetType(), this.GetHashCode()));
            base.Dispose(disposing);
        }

        partial void SwitchCameraButtonTouched(UIBarButtonItem sender)
        {
            businessLogic.SwitchCamera();
        }

        partial void CloseButtonTouched(UIBarButtonItem sender)
        {
            this.DismissViewController(true, null);
        }
    }
}