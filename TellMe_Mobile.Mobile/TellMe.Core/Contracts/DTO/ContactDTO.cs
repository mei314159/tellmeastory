﻿using Newtonsoft.Json;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("Contacts")]
    public class ContactDTO
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }

        public string PhoneNumber { get; set; }

        public long PhoneNumberDigits { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public bool IsAppUser => UserId != null;
    }
}