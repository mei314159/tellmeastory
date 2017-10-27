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


        [SQLiteNetExtensions.Attributes.TextBlob("MembersBlobbed")]
        public virtual ICollection<TribeMemberDTO> Members { get; set; }


        public string StatusBlobbed { get; set; }
        public string MembersBlobbed { get; set; }
    }
}