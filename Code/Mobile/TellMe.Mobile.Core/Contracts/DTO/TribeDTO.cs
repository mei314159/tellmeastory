﻿using System;
using System.Collections.Generic;

namespace TellMe.Mobile.Core.Contracts.DTO
{
    [SQLite.Table("Tribes")]
    public class TribeDTO
    {
        [SQLite.PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public string CreatorPictureUrl { get; set; }

        public int MembersCount { get; set; }
        public int EventsCount { get; set; }
        public int StoriesCount { get; set; }

        [SQLiteNetExtensions.Attributes.TextBlob("StatusBlobbed")]
        public TribeMemberStatus MembershipStatus { get; set; }


        [SQLiteNetExtensions.Attributes.TextBlob("MembersBlobbed")]
        public virtual List<TribeMemberDTO> Members { get; set; }


        public string StatusBlobbed { get; set; }
        public string MembersBlobbed { get; set; }
    }
}