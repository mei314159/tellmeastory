﻿using System;

using Foundation;
using TellMe.Core.Contracts.DTO;
using UIKit;

namespace TellMe.iOS.Views.Cells
{
    public partial class ReceiversListCell : UICollectionViewCell
    {
        private StoryReceiverDTO receiver;
        private UIImage defaultPicture;

        public static readonly NSString Key = new NSString("ReceiversListCell");
        public static readonly UINib Nib;

        static ReceiversListCell()
        {
            Nib = UINib.FromName("ReceiversListCell", NSBundle.MainBundle);
        }

        protected ReceiversListCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.defaultPicture = UIImage.FromBundle("UserPic");
        }

        public StoryReceiverDTO Receiver
        {
            get
            {
                return receiver;
            }

            set
            {
                receiver = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            ProfilePicture.SetPictureUrl(receiver.ReceiverPictureUrl, defaultPicture);
        }
    }
}