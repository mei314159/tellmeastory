using Foundation;
using System;
using AVFoundation;

namespace TellMe.iOS
{
    public class OutputDelegate : AVCaptureFileOutputRecordingDelegate
    {
        public event Action<DateTime> Started;
        public event Action Stopped;

        public override void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
        {
            Stopped.Invoke();
        }

        public override void DidStartRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections)
        {
            Started.Invoke(DateTime.Now);
        }
    }
}