﻿using System.Collections.Generic;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts.UI.Views
{
    public delegate void StorytellerSelectedEventHandler(ICollection<ContactDTO> selectedContacts);
}