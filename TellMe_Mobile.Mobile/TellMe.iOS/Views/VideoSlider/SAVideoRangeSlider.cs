using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using UIKit;

namespace TellMe.iOS.Views.VideoSlider
{
    public delegate void GestureStateEndedLeftPositionHandler(SAVideoRangeSlider videoRange, nfloat leftPosition, nfloat rightPosition);
    [Register("SAVideoRangeSlider")]
    public class SAVideoRangeSlider : UIView
    {
        AVAssetImageGenerator imageGenerator;
        UIView bgView;
        UIView centerView;
        NSUrl videoUrl;
        SASliderLeft leftThumb;
        SASliderRight rightThumb;
        nfloat FrameWidth;
        nfloat durationSeconds;
        SAResizableBubble popoverBubble;
        private nfloat leftPosition;
        private nfloat rightPosition;
        private nint minGap;
        private nint maxGap;
        const float SLIDERBORDERSSIZE = 6.0f;
        const float BGVIEWBORDERSSIZE = 3.0f;
        private UILabel bubleText;
        private UIView topBorder;
        private UIView bottomBorder;

        public event GestureStateEndedLeftPositionHandler didGestureStateEndedLeftPosition;
        public event GestureStateEndedLeftPositionHandler didChangeLeftPosition;

        public SAVideoRangeSlider(CGRect Frame, NSUrl videoUrl) : base(Frame)
        {
            FrameWidth = Frame.Width;

            int thumbWidth = (int)Math.Ceiling(Frame.Width * 0.05);

            bgView = new UIControl(new CGRect(thumbWidth - BGVIEWBORDERSSIZE, 0, Frame.Width - (thumbWidth * 2) + BGVIEWBORDERSSIZE * 2, Frame.Height));
            bgView.Layer.BorderColor = UIColor.Gray.CGColor;
            bgView.Layer.BorderWidth = BGVIEWBORDERSSIZE;
            this.AddSubview(bgView);
            this.videoUrl = videoUrl;


            topBorder = new UIView(new CGRect(0, 0, Frame.Width, SLIDERBORDERSSIZE));
            topBorder.BackgroundColor = UIColor.FromRGBA(0.996f, 0.951f, 0.502f, 1);
            this.AddSubview(topBorder);


            bottomBorder = new UIView(new CGRect(0, Frame.Height - SLIDERBORDERSSIZE, Frame.Width, SLIDERBORDERSSIZE));
            bottomBorder.BackgroundColor = UIColor.FromRGBA(0.992f, 0.902f, 0.004f, 1);
            this.AddSubview(bottomBorder);


            leftThumb = new SASliderLeft(new CGRect(0, 0, thumbWidth, Frame.Height));
            leftThumb.ContentMode = UIViewContentMode.Left;
            leftThumb.UserInteractionEnabled = false;
            leftThumb.ClipsToBounds = false;
            leftThumb.BackgroundColor = UIColor.Clear;
            leftThumb.Layer.BorderWidth = 0;
            this.AddSubview(leftThumb);

            var leftPan = new UIPanGestureRecognizer(HandleLeftPan);
            leftThumb.AddGestureRecognizer(leftPan);

            rightThumb = new SASliderRight(new CGRect(0, 0, thumbWidth, Frame.Height));

            rightThumb.ContentMode = UIViewContentMode.Right;
            rightThumb.UserInteractionEnabled = false;
            rightThumb.ClipsToBounds = false;
            rightThumb.BackgroundColor = UIColor.Clear;
            this.AddSubview(rightThumb);

            var rightPan = new UIPanGestureRecognizer(HandleRightPan);
            rightThumb.AddGestureRecognizer(rightPan);

            this.rightPosition = Frame.Width;
            this.leftPosition = 0;

            centerView = new UIView(new CGRect(0, 0, Frame.Width, Frame.Height));
            centerView.BackgroundColor = UIColor.Clear;
            this.AddSubview(centerView);

            var centerPan = new UIPanGestureRecognizer(HandleCenterPan);
            centerView.AddGestureRecognizer(centerPan);


            popoverBubble = new SAResizableBubble(new CGRect(0, -50, 100, 50));
            popoverBubble.Alpha = 0;
            popoverBubble.BackgroundColor = UIColor.Clear;
            this.AddSubview(popoverBubble);


            this.bubleText = new UILabel(popoverBubble.Frame);
            bubleText.Font = UIFont.BoldSystemFontOfSize(20);
            bubleText.BackgroundColor = UIColor.Clear;
            bubleText.TextColor = UIColor.Black;
            bubleText.TextAlignment = UITextAlignment.Center;

            popoverBubble.AddSubview(bubleText);
            this.getMovieFrame();
        }

        private void SetPopoverBubbleSize(nfloat width, nfloat height)
        {

            CGRect currentFrame = popoverBubble.Frame;
            currentFrame.Width = width;
            currentFrame.Height = height;
            currentFrame.Y = -height;
            popoverBubble.Frame = currentFrame;

            currentFrame.X = 0;
            currentFrame.Y = 0;
            bubleText.Frame = currentFrame;

        }


        private void SetMaxGap(nint maximalGap)
        {
            leftPosition = 0;
            rightPosition = FrameWidth * maximalGap / durationSeconds;
            this.maxGap = maximalGap;
        }

        private void SetMinGap(nint minimalGap)
        {
            leftPosition = 0;
            rightPosition = FrameWidth * minimalGap / durationSeconds;
            this.minGap = minimalGap;
        }


        private void DelegateNotification()
        {
            didChangeLeftPosition?.Invoke(this, leftPosition, rightPosition);
        }

        public override void LayoutSubviews()
        {
            var inset = leftThumb.Frame.Width / 2;

            leftThumb.Center = new CGPoint(leftPosition + inset, leftThumb.Frame.Height / 2);

            rightThumb.Center = new CGPoint(rightPosition - inset, rightThumb.Frame.Height / 2);

            topBorder.Frame = new CGRect(leftThumb.Frame.X + leftThumb.Frame.Width, 0, rightThumb.Frame.X - leftThumb.Frame.X - leftThumb.Frame.Width / 2, SLIDERBORDERSSIZE);

            bottomBorder.Frame = new CGRect(leftThumb.Frame.X + leftThumb.Frame.Width, bgView.Frame.Height - SLIDERBORDERSSIZE, rightThumb.Frame.X - leftThumb.Frame.X - leftThumb.Frame.Width / 2, SLIDERBORDERSSIZE);


            centerView.Frame = new CGRect(leftThumb.Frame.X + leftThumb.Frame.Width, centerView.Frame.Y, rightThumb.Frame.X - leftThumb.Frame.X - leftThumb.Frame.Width, centerView.Frame.Height);

            var frame = popoverBubble.Frame;
            frame.X = centerView.Frame.X + centerView.Frame.Width / 2 - Frame.Width / 2;
            popoverBubble.Frame = Frame;
        }

        private void HandleLeftPan(UIPanGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed)
            {

                var translation = gesture.TranslationInView(this);

                leftPosition += translation.X;
                if (leftPosition < 0)
                {
                    leftPosition = 0;
                }

                if (
                    (rightPosition - leftPosition <= leftThumb.Frame.Width + rightThumb.Frame.Width) ||
                    ((this.maxGap > 0) && (this.rightPosition - this.leftPosition > this.maxGap)) ||
                    ((this.minGap > 0) && (this.rightPosition - this.leftPosition < this.minGap))
                    )
                {
                    leftPosition -= translation.X;
                }


                gesture.SetTranslation(CGPoint.Empty, this);
                this.SetNeedsLayout();
                this.DelegateNotification();

            }

            popoverBubble.Alpha = 1;

            this.SetTimeLabel();

            if (gesture.State == UIGestureRecognizerState.Ended)
            {
                this.HideBubble(popoverBubble);
            }
        }


        private void HandleRightPan(UIPanGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed)
            {
                CGPoint translation = gesture.TranslationInView(this);
                rightPosition += translation.X;
                if (rightPosition < 0)
                {
                    rightPosition = 0;
                }

                if (rightPosition > FrameWidth)
                {
                    rightPosition = FrameWidth;
                }

                if (rightPosition - leftPosition <= 0)
                {
                    rightPosition -= translation.X;
                }

                if ((rightPosition - leftPosition <= leftThumb.Frame.Width + rightThumb.Frame.Width) ||
                    ((this.maxGap > 0) && (this.rightPosition - this.leftPosition > this.maxGap)) ||
                    ((this.minGap > 0) && (this.rightPosition - this.leftPosition < this.minGap)))
                {
                    rightPosition -= translation.X;
                }



                gesture.SetTranslation(CGPoint.Empty, this);
                this.SetNeedsLayout();
                this.DelegateNotification();
            }

            popoverBubble.Alpha = 1;

            this.SetTimeLabel();

            if (gesture.State == UIGestureRecognizerState.Ended)
            {
                this.HideBubble(popoverBubble);
            }
        }


        void HandleCenterPan(UIPanGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Began || gesture.State == UIGestureRecognizerState.Changed)
            {

                CGPoint translation = gesture.TranslationInView(this);

                leftPosition += translation.X;
                rightPosition += translation.X;

                if (rightPosition > FrameWidth || leftPosition < 0)
                {
                    leftPosition -= translation.X;
                    rightPosition -= translation.X;
                }



                gesture.SetTranslation(CGPoint.Empty, this);
                this.SetNeedsLayout();
                this.DelegateNotification();

            }

            popoverBubble.Alpha = 1;

            this.SetTimeLabel();

            if (gesture.State == UIGestureRecognizerState.Ended)
            {
                this.HideBubble(popoverBubble);
            }
        }

        void getMovieFrame()
        {
            AVAsset myAsset = new AVUrlAsset(videoUrl);
            this.imageGenerator = new AVAssetImageGenerator(myAsset);

            if (this.IsRetina())
            {
                this.imageGenerator.MaximumSize = new CGSize(bgView.Frame.Size.Width * 2, bgView.Frame.Height * 2);
            }
            else
            {
                this.imageGenerator.MaximumSize = new CGSize(bgView.Frame.Width, bgView.Frame.Height);
            }

            int picWidth = 20;

            // First image
            NSError error;
            CMTime actualTime;
            var halfWayImage = this.imageGenerator.CopyCGImageAtTime(CMTime.Zero, out actualTime, out error);
            if (halfWayImage != null)
            {
                UIImage videoScreen;
                if (this.IsRetina())
                {
                    videoScreen = new UIImage(halfWayImage, 2.0f, UIImageOrientation.Up);
                }
                else
                {
                    videoScreen = new UIImage(halfWayImage);
                }
                UIImageView tmp = new UIImageView(videoScreen);
                CGRect rect = new CGRect(tmp.Frame.Location, new CGSize(picWidth, tmp.Frame.Height));
                tmp.Frame = rect;
                bgView.AddSubview(tmp);
                picWidth = (int)tmp.Frame.Size.Width;
                halfWayImage.Dispose();
            }


            durationSeconds = (nfloat)myAsset.Duration.Seconds;

            int picsCnt = (int)Math.Ceiling(bgView.Frame.Width / picWidth);

            NSMutableArray allTimes = new NSMutableArray();

            int time4Pic = 0;

            int prefreWidth = 0;
            for (int i = 1, ii = 1; i < picsCnt; i++)
            {
                time4Pic = i * picWidth;

                CMTime timeFrame = CMTime.FromSeconds(durationSeconds * time4Pic / bgView.Frame.Width, 600);

                allTimes.Add(NSValue.FromCMTime(timeFrame));

                halfWayImage = this.imageGenerator.CopyCGImageAtTime(timeFrame, out actualTime, out error);

                UIImage videoScreen;
                if (this.IsRetina())
                {
                    videoScreen = new UIImage(halfWayImage, 2.0f, UIImageOrientation.Up);
                }
                else
                {
                    videoScreen = new UIImage(halfWayImage);
                }

                UIImageView tmp = new UIImageView(videoScreen);

                CGRect currentFrame = tmp.Frame;
                currentFrame.X = ii * picWidth;

                currentFrame.Size = new CGSize(picWidth, currentFrame.Height);
                prefreWidth += (int)currentFrame.Size.Width;

                if (i == picsCnt - 1)
                {
                    currentFrame.Size = new CGSize(currentFrame.Size.Width - 6, currentFrame.Size.Height);
                }

                tmp.Frame = currentFrame;
                int all = (int)((ii + 1) * tmp.Frame.Width);

                if (all > bgView.Frame.Width)
                {
                    var delta = all - bgView.Frame.Width;
                    currentFrame.Size = new CGSize(currentFrame.Size.Width - delta, currentFrame.Size.Height);
                }

                ii++;

                InvokeOnMainThread(() => bgView.AddSubview(tmp));


                halfWayImage.Dispose();
            }
        }



        private nfloat LeftPosition()
        {
            return leftPosition * durationSeconds / FrameWidth;
        }


        private nfloat RightPosition()
        {
            return rightPosition * durationSeconds / FrameWidth;
        }



        private void HideBubble(UIView popover)
        {
            UIView.Animate(0.4, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.AllowUserInteraction, () =>
            {
                popoverBubble.Alpha = 0;
            }, null);

            didGestureStateEndedLeftPosition?.Invoke(this, leftPosition, rightPosition);
        }


        private void SetTimeLabel()
        {
            this.bubleText.Text = this.TrimIntervalStr();
            //NSLog(this timeDuration1));
            //NSLog(this timeDuration));
        }


        private NSString TrimDurationStr()
        {
            var delta = Math.Floor(this.rightPosition - this.leftPosition);
            return NSString.LocalizedFormat(@"%d", delta);
        }


        private NSString TrimIntervalStr()
        {
            NSString from = this.TimeToStr(this.leftPosition);
            NSString to = this.TimeToStr(this.rightPosition);
            return NSString.LocalizedFormat(@"%@ - %@", from, to);
        }

        private NSString TimeToStr(nfloat time)
        {
            // time - seconds
            var min = Math.Floor(time / 60);
            var sec = Math.Floor(time - min * 60);
            var minStr = NSString.LocalizedFormat(min >= 10 ? @"%i" : @"0%i", min);
            var secStr = NSString.LocalizedFormat(sec >= 10 ? @"%i" : @"0%i", sec);
            return NSString.LocalizedFormat(@"%@:%@", minStr, secStr);
        }

        private bool IsRetina()
        {
            return UIScreen.MainScreen.Scale == 2.0;
        }
    }
}
