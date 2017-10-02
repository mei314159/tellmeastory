using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace TellMe.iOS.Views.CustomPicker
{
    public delegate void RowSelected(UIPickerView pickerView, nint row, nint component);

    public class CustomPickerModel : UIPickerViewModel
    {
        public event RowSelected ItemSelected;

        public CustomPickerModel(List<string> itemsList)
        {
            Items = itemsList;
        }

        public List<string> Items { get; private set; }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Items.Count;
        }

        public override UIView GetView(UIPickerView picker, nint row, nint component, UIView view)
        {
            var label = new UILabel(new CGRect(0, 0, 300, 37))
            {
                BackgroundColor = UIColor.Clear,
                Text = Items[(int)row],
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.BoldSystemFontOfSize(22.0f)
            };

            return label;
        }

        public override void Selected (UIPickerView pickerView, nint row, nint component)
        {
            ItemSelected?.Invoke (pickerView, row, component);
        }
    }
}

