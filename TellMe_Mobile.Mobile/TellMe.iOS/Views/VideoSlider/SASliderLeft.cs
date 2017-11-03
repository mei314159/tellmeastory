//
//  SASliderLeft.cs
//  TellMe.iOS
//
//  Created by Vitay Dubnovitsky on 10/30/17.
//  Copyright © 2017 Brainworks LLC. All rights reserved.
//
//  Generated by PaintCode
//  http://www.paintcodeapp.com
//



using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace TellMe.iOS
{
    [Register("SASliderLeft")]
    public class SASliderLeft : UIView
    {
        public SASliderLeft(CGRect frame) : base(frame)
        {
        }

        public override void Draw(CGRect rect)
        {
            DrawCanvas1(rect);
        }

        //// Drawing Methods

        public static void DrawCanvas1(CGRect bubbleFrame)
        {
            //// General Declarations
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var context = UIGraphics.GetCurrentContext();

            //// Color Declarations
            var color5 = UIColor.FromRGBA(0.992f, 0.902f, 0.004f, 1.000f);
            var gradientColor2 = UIColor.FromRGBA(1.000f, 1.000f, 1.000f, 1.000f);
            var color6 = UIColor.FromRGBA(0.196f, 0.161f, 0.047f, 1.000f);

            //// Gradient Declarations
            var gradient3Colors = new CGColor[] { gradientColor2.CGColor, BlendedColor(gradientColor2, 0.5f, color5).CGColor, color5.CGColor };
            var gradient3Locations = new nfloat[] { 0.0f, 0.0f, 0.49f };
            var gradient3 = new CGGradient(colorSpace, gradient3Colors, gradient3Locations);

            //// Rounded Rectangle Drawing
            var roundedRectangleRect = new CGRect(bubbleFrame.GetMinX(), bubbleFrame.GetMinY(), bubbleFrame.Width, bubbleFrame.Height);
            var roundedRectanglePath = UIBezierPath.FromRoundedRect(roundedRectangleRect, UIRectCorner.TopLeft | UIRectCorner.BottomLeft, new CGSize(5.0f, 5.0f));
            roundedRectanglePath.ClosePath();
            context.SaveState();
            roundedRectanglePath.AddClip();
            context.DrawLinearGradient(gradient3,
                new CGPoint(roundedRectangleRect.GetMidX(), roundedRectangleRect.GetMinY()),
                new CGPoint(roundedRectangleRect.GetMidX(), roundedRectangleRect.GetMaxY()),
                0);
            context.RestoreState();
            UIColor.Clear.SetStroke();
            roundedRectanglePath.LineWidth = 0.5f;
            roundedRectanglePath.Stroke();


            //// Bezier 3 Drawing
            var bezier3Path = new UIBezierPath();
            bezier3Path.MoveTo(new CGPoint(bubbleFrame.GetMinX() + 0.42806f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height));
            bezier3Path.AddCurveToPoint(new CGPoint(bubbleFrame.GetMinX() + 0.42806f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.74629f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.42806f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.42806f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height));
            bezier3Path.AddLineTo(new CGPoint(bubbleFrame.GetMinX() + 0.35577f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.74629f * bubbleFrame.Height));
            bezier3Path.AddCurveToPoint(new CGPoint(bubbleFrame.GetMinX() + 0.35577f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.35577f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.35577f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height));
            bezier3Path.AddLineTo(new CGPoint(bubbleFrame.GetMinX() + 0.42806f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height));
            bezier3Path.ClosePath();
            color6.SetFill();
            bezier3Path.Fill();


            //// Bezier Drawing
            var bezierPath = new UIBezierPath();
            bezierPath.MoveTo(new CGPoint(bubbleFrame.GetMinX() + 0.66944f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height));
            bezierPath.AddCurveToPoint(new CGPoint(bubbleFrame.GetMinX() + 0.66944f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.74629f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.66944f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.66944f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height));
            bezierPath.AddLineTo(new CGPoint(bubbleFrame.GetMinX() + 0.59715f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.74629f * bubbleFrame.Height));
            bezierPath.AddCurveToPoint(new CGPoint(bubbleFrame.GetMinX() + 0.59715f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.59715f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height), new CGPoint(bubbleFrame.GetMinX() + 0.59715f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.69415f * bubbleFrame.Height));
            bezierPath.AddLineTo(new CGPoint(bubbleFrame.GetMinX() + 0.66944f * bubbleFrame.Width, bubbleFrame.GetMinY() + 0.22486f * bubbleFrame.Height));
            bezierPath.ClosePath();
            color6.SetFill();
            bezierPath.Fill();
        }




        static UIColor BlendedColor(UIColor color1, nfloat fraction, UIColor color2)
        {
            var rgba1 = new nfloat[4];
            var rgba2 = new nfloat[4];

            color1.GetRGBA(out rgba1[0], out rgba1[1], out rgba1[2], out rgba1[3]);
            color2.GetRGBA(out rgba2[0], out rgba2[1], out rgba2[2], out rgba2[3]);

            return new UIColor(
                rgba1[0] * (1 - fraction) + rgba2[0] * fraction,
                rgba1[1] * (1 - fraction) + rgba2[1] * fraction,
                rgba1[2] * (1 - fraction) + rgba2[2] * fraction,
                rgba1[3] * (1 - fraction) + rgba2[3] * fraction);
        }
    }
}
