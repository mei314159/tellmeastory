using System;
using System.Collections.Generic;

namespace TellMe.DAL.Types.Domain
{
    public class Tribe : EntityBase<int>
    {
        public string Name { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public virtual ICollection<TribeMember> Members { get; set; }
    }
}