using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class ApplicationUser : IdentityUser, IEntityBase<string>
    {
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
