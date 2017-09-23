using System.Collections.Generic;
using TellMe.DAL.Contracts.DTO;

namespace TellMe.Web.DTO
{
    public class SynchronizeContactsDTO
    {
        public List<PhoneContactDTO> Contacts { get; set; }
    }
}
