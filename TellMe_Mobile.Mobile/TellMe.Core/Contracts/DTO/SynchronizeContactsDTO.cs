using System.Collections.Generic;

namespace TellMe.Core.Contracts.DTO
{
    public class SynchronizeContactsDTO
    {
        public List<PhoneContactDTO> Contacts { get; set; }
    }
}