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
            var contactsQuery = _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

            var result = await (from contact in contactsQuery
                                join user in usersQuery on contact.PhoneNumberDigits equals user.PhoneNumberDigits into gj
                                from x in gj.DefaultIfEmpty()
                                select new ContactDTO
                                {
                                    Id = contact.Id,
                                    PhoneNumber = contact.PhoneNumber,
                                    PhoneNumberDigits = contact.PhoneNumberDigits,
                                    Name = contact.Name,
                                    UserId = x != null ? x.Id : null
                                }).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public async Task SaveContactsAsync(string userId, IReadOnlyCollection<PhoneContactDTO> contacts)
        {
            var userContacts = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.PhoneNumberDigits)
            .ToListAsync()
            .ConfigureAwait(false);

            var regex = new Regex(Constants.PhoneNumberCleanupRegex);
            var newContacts = contacts
            .Select(x => new { x.Name, PhoneNumber = x.PhoneNumber, PhoneNumberDigits = long.Parse(regex.Replace(x.PhoneNumber, string.Empty)) })
            .Where(x => !userContacts.Contains(x.PhoneNumberDigits));
            foreach (var number in newContacts)
            {
                var entity = new Contact
                {
                    UserId = userId,
                    PhoneNumber = number.PhoneNumber,
                    PhoneNumberDigits = number.PhoneNumberDigits,
                    Name = number.Name
                };

                _contactRepository.Save(entity);
            }

            _contactRepository.Commit();
        }
    }
}