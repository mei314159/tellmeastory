﻿using System;
using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public interface ICreateEventView : IView
    {
        EventDTO Event { get; set; }

        void Display(EventDTO eventDTO);
        void DisplayMembers();
        void ShowSuccessMessage(string message, Action complete = null);
        void Close(EventDTO deletedEventDTO);
    }
}