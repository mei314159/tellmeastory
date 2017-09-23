using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.DTO;

namespace TellMe.Core.Services
{
    public class ContactsService : BaseDataService
    {
        
        public ContactsService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public async Task<ServiceResult> SynchronizeContactsAsync(IReadOnlyCollection<PhoneContactDTO> contacts)
        {
            var result = await this.PostAsync<List<ContactDTO>>("contacts/synchronize", new SynchronizeContactsDTO
            {
                Contacts = contacts.ToList()
            }).ConfigureAwait(false);

            var serviceResult = new ServiceResult(null, result);
            return serviceResult;
        }

        public async Task<Result<List<ContactDTO>>> GetContactsAsync()
        {
            var result = await this.GetAsync<List<ContactDTO>>("contacts").ConfigureAwait(false);
            return result;
        }
    }
}
