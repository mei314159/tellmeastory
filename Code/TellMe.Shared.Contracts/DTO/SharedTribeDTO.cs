using System;
using System.Collections.Generic;
using TellMe.Shared.Contracts.Enums;

namespace TellMe.Shared.Contracts.DTO
{
    public class SharedTribeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public string CreatorPictureUrl { get; set; }
        
        public int MembersCount { get; set; }
        public int EventsCount { get; set; }
        public int StoriesCount { get; set; }
        
        public TribeMemberStatus MembershipStatus { get; set; }

        public List<SharedTribeMemberDTO> Members { get; set; }
    }
}