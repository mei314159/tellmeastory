using System;
using System.Collections.Generic;

namespace TellMe.Core.Contracts.DTO
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

        [SQLiteNetExtensions.Attributes.TextBlob("StatusBlobbed")]
        public TribeMemberStatus MembershipStatus { get; set; }

        public virtual ICollection<StorytellerDTO> Members { get; set; }


        public string StatusBlobbed { get; set; }
    }
}