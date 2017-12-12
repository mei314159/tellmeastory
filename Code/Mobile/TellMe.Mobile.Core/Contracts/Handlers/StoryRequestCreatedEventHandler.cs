﻿using System.Collections.Generic;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.Handlers
{
    public delegate void StoryRequestCreatedEventHandler(RequestStoryDTO dto, ICollection<ContactDTO> recipients);
}