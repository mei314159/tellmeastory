using System;
using System.Collections.Generic;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.DTO
{
    public class TribeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public string CreatorId { get; set; }

        public string CreatorName { get; set; }

        public string CreatorPictureUrl { get; set; }
        
        public TribeMemberStatus MembershipStatus { get; set; }

        public List<StorytellerDTO> Members { get; set; }
    }
}
