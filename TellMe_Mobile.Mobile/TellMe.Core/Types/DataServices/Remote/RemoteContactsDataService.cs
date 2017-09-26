using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class RemoteContactsDataService : BaseDataService
    {
        
        public RemoteContactsDataService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public async Task<Result> SynchronizeContactsAsync(IReadOnlyCollection<PhoneContactDTO> contacts)
        {
            var result = await this.PostAsync<List<ContactDTO>>("contacts/synchronize", new SynchronizeContactsDTO
            {
                Contacts = contacts.ToList()
            }).ConfigureAwait(false);

            return result;
        }

        public async Task<Result<List<ContactDTO>>> GetContactsAsync()
        {
            var result = await this.GetAsync<List<ContactDTO>>("contacts").ConfigureAwait(false);
            return result;
        }
    }
}
