﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class ApplicationUser : IdentityUser, IEntityBase<string>
    {
        public long PhoneNumberDigits { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

        public virtual ICollection<PushNotificationClient> PushNotificationClients { get; set; }

        public virtual ICollection<Story> SentStories { get; set; }

        public virtual ICollection<Story> ReceivedStories { get; set; }
        
        public int PhoneCountryCode { get; set; }

        [MaxLength(2)]
        public string CountryCode { get; set; }
    }
}
