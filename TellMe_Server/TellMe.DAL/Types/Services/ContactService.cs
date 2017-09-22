using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TellMe.DAL.Contracts.Repositories;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using Microsoft.EntityFrameworkCore;
using TellMe.DAL.Contracts.DTO;
using AutoMapper.QueryableExtensions;
using System.Text.RegularExpressions;

namespace TellMe.DAL.Types.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact, int> _contactRepository;
        private readonly IRepository<ApplicationUser, string> _userRepository;

        public ContactService(IRepository<Contact, int> contactRepository, IRepository<ApplicationUser, string> userRepository)
        {
            _contactRepository = contactRepository;
            _userRepository = userRepository;
        }

        public async Task<ICollection<ContactDTO>> GetAllAsync(string userId)
        {
            var usersQuery = _userRepository.GetQueryable().AsNoTracking();
            var result = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Join(usersQuery, x => x.PhoneNumber, x => x.PhoneNumber, (c, u) => new ContactDTO
            {
                Id = c.Id,
                PhoneNumber = c.PhoneNumber,
                UserId = u.Id
            }).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task SaveContactsAsync(string userId, IReadOnlyCollection<ContactDTO> contacts)
        {
            var userContacts = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.PhoneNumber)
            .ToListAsync()
            .ConfigureAwait(false);

            var regex = new Regex(Constants.PhoneNumberCleanupRegex);
            var newContacts = contacts.Select(x => regex.Replace(x.PhoneNumber, string.Empty)).Where(x=> !userContacts.Contains(x));
            foreach (var number in newContacts)
            {
                var entity = new Contact
                {
                    UserId = userId,
                    PhoneNumber = number
                };

                _contactRepository.Save(entity);
            }

            _contactRepository.Commit();
        }
    }
}