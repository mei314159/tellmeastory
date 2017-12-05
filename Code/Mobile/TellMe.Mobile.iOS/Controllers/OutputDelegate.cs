using System;
using AVFoundation;
using Foundation;

namespace TellMe.iOS.Controllers
{
    public class OutputDelegate : AVCaptureFileOutputRecordingDelegate
    {
        public event Action<DateTime> Started;
        public event Action Stopped;

        public override void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl,
            NSObject[] connections, NSError error)
        {
            Stopped?.Invoke();
        }

        public override void DidStartRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl,
            NSObject[] connections)
        {
            Started?.Invoke(DateTime.Now);
        }
    }
}