﻿namespace TellMe.Core.Contracts.UI.Components
{
    public interface ITextInput
    {
        string Text { get; set; }
        bool Enabled { get; set; }
    }
}