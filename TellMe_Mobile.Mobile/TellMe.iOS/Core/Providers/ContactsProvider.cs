using System;
using System.Collections.Generic;
using Contacts;
using TellMe.Core.Contracts.Providers;
using TellMe.Core.Contracts.DTO;

namespace TellMe.iOS.Core.Providers
{
    public class ContactsProvider : IContactsProvider
    {
        public IReadOnlyCollection<PhoneContactDTO> GetContacts()
        {
            var keysTOFetch = new[] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers };

            CNContact[] contactList;
            var ContainerId = new CNContactStore().DefaultContainerIdentifier;
            using (var predicate = CNContact.GetPredicateForContactsInContainer(ContainerId))
            {
                using (var store = new CNContactStore())
                {
                    contactList = store.GetUnifiedContacts(predicate, keysTOFetch, out Foundation.NSError error);
                }
            }

            var contacts = new List<PhoneContactDTO>();

            foreach (var item in contactList)
            {
                var numbers = item.PhoneNumbers;
                if (numbers != null)
                {
                    foreach (var number in numbers)
                    {
                        contacts.Add(new PhoneContactDTO
                        {
                            Name = $"{item.GivenName} {item.FamilyName}",
                            //LocalId = number.Value.ValueForKey(new NSString("digits")).ToString(),
                            PhoneNumber = number.Value.StringValue
                        });
                    }
                }
            }

            return contacts;
        }

        public Permissions GetPermissions()
        {
            switch (CNContactStore.GetAuthorizationStatus(CNEntityType.Contacts))
            {
                case CNAuthorizationStatus.Restricted:
                    return Permissions.Restricted;
                case CNAuthorizationStatus.Denied:
                    return Permissions.Denied;
                case CNAuthorizationStatus.Authorized:
                    return Permissions.Authorized;
                default:
                    return Permissions.NotDetermined;
            }
        }
    }
}
