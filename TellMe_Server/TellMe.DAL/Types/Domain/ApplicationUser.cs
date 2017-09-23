using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TellMe.DAL.Contracts.Domain;

namespace TellMe.DAL.Types.Domain
{
    public class ApplicationUser : IdentityUser, IEntityBase<string>
    {
        public long PhoneNumberDigits { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
