using System.Collections.Generic;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.DTO;
using TellMe.DAL.Types.Domain;

namespace TellMe.DAL.Contracts.Services
{
    public interface IContactService : IService
    {
        Task<ICollection<ContactDTO>> GetAllAsync(string userId);

        Task SaveContactsAsync(string userId, IReadOnlyCollection<PhoneContactDTO> contacts);
    }    
}