using System.Collections.Generic;

namespace TellMe.Mobile.Core.Contracts.DTO
{
    public class SynchronizeContactsDTO
    {
        public List<PhoneContactDTO> Contacts { get; set; }
    }
}