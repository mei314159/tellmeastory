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
                                join user in usersQuery
                                on contact.PhoneNumberDigits equals user.PhoneNumberDigits into gj
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
            var user = await _userRepository
            .GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId)
            .ConfigureAwait(false);
            var userContacts = await _contactRepository
            .GetQueryable()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.PhoneNumberDigits)
            .ToListAsync()
            .ConfigureAwait(false);

            bool isUS = user.CountryCode == "US";
            var regex = new Regex(Constants.PhoneNumberCleanupRegex);
            var newContacts = contacts
            .Select(x =>
            {
                var digitsString = regex.Replace(x.PhoneNumber, string.Empty);
                if (!digitsString.StartsWith("+") 
                    && (!isUS || !PhoneCodes.USPhoneCodes.Any(digitsString.StartsWith)))
                {
                    digitsString = user.PhoneCountryCode.ToString() + digitsString;
                }

                return new
                {
                    x.Name,
                    PhoneNumber = x.PhoneNumber,
                    PhoneNumberDigits = long.Parse(digitsString)
                };
            })
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

    public static class PhoneCodes
    {
        public static HashSet<string> USPhoneCodes = new HashSet<string> {
            "1800",
            "1855",
            "1888",
            "1277",
            "1325",
            "1330",
            "1234",
            "1518",
            "1229",
            "1957",
            "1505",
            "1320",
            "1730",
            "1618",
            "1657",
            "1909",
            "1752",
            "1714",
            "1907",
            "1734",
            "1278",
            "1703",
            "1571",
            "1828",
            "1606",
            "1404",
            "1770",
            "1678",
            "1470",
            "1609",
            "1762",
            "1706",
            "1331",
            "1737",
            "1512",
            "1667",
            "1443",
            "1410",
            "1225",
            "1425",
            "1360",
            "1240",
            "1610",
            "1484",
            "1835",
            "1406",
            "1228",
            "1659",
            "1205",
            "1952",
            "1208",
            "1857",
            "1617",
            "1802",
            "1631",
            "1203",
            "1475",
            "1718",
            "1347",
            "1979",
            "1818",
            "1747",
            "1856",
            "1239",
            "1319",
            "1447",
            "1217",
            "1843",
            "1681",
            "1304",
            "1980",
            "1704",
            "1423",
            "1872",
            "1773",
            "1312",
            "1413",
            "1708",
            "1464",
            "1283",
            "1513",
            "1931",
            "1440",
            "1216",
            "1573",
            "1803",
            "1614",
            "1380",
            "1925",
            "1361",
            "1214",
            "1972",
            "1469",
            "1764",
            "1650",
            "1276",
            "1563",
            "1937",
            "1386",
            "1940",
            "1720",
            "1303",
            "1313",
            "1679",
            "1620",
            "1218",
            "1715",
            "1534",
            "1848",
            "1732",
            "1915",
            "1908",
            "1607",
            "1814",
            "1760",
            "1442",
            "1541",
            "1458",
            "1812",
            "1701",
            "1910",
            "1810",
            "1954",
            "1754",
            "1479",
            "1260",
            "1682",
            "1817",
            "1559",
            "1352",
            "1409",
            "1219",
            "1970",
            "1616",
            "1231",
            "1920",
            "1274",
            "1336",
            "1864",
            "1254",
            "1985",
            "1959",
            "1860",
            "1516",
            "1808",
            "1832",
            "1713",
            "1281",
            "1938",
            "1256",
            "1936",
            "1317",
            "1515",
            "1949",
            "1769",
            "1601",
            "1731",
            "1904",
            "1551",
            "1201",
            "1870",
            "1913",
            "1975",
            "1816",
            "1308",
            "1262",
            "1845",
            "1865",
            "1337",
            "1765",
            "1863",
            "1717",
            "1740",
            "1517",
            "1307",
            "1956",
            "1575",
            "1702",
            "1580",
            "1859",
            "1501",
            "1562",
            "1323",
            "1310",
            "1213",
            "1502",
            "1978",
            "1351",
            "1806",
            "1434",
            "1339",
            "1781",
            "1478",
            "1608",
            "1603",
            "1507",
            "1660",
            "1641",
            "1830",
            "1901",
            "1786",
            "1305",
            "1414",
            "1612",
            "1251",
            "1334",
            "1630",
            "1615",
            "1724",
            "1504",
            "1917",
            "1646",
            "1212",
            "1973",
            "1862",
            "1716",
            "1510",
            "1341",
            "1432",
            "1405",
            "1531",
            "1402",
            "1927",
            "1689",
            "1407",
            "1321",
            "1269",
            "1364",
            "1270",
            "1445",
            "1267",
            "1215",
            "1623",
            "1602",
            "1480",
            "1878",
            "1412",
            "1763",
            "1626",
            "1248",
            "1772",
            "1971",
            "1503",
            "1207",
            "1401",
            "1719",
            "1919",
            "1984",
            "1530",
            "1775",
            "1804",
            "1951",
            "1540",
            "1585",
            "1309",
            "1815",
            "1779",
            "1252",
            "1916",
            "1989",
            "1831",
            "1801",
            "1385",
            "1210",
            "1935",
            "1858",
            "1619",
            "1628",
            "1415",
            "1408",
            "1669",
            "1805",
            "1661",
            "1424",
            "1627",
            "1369",
            "1707",
            "1941",
            "1906",
            "1912",
            "1570",
            "1206",
            "1564",
            "1318",
            "1301",
            "1227",
            "1712",
            "1605",
            "1574",
            "1509",
            "1417",
            "1636",
            "1435",
            "1314",
            "1557",
            "1651",
            "1727",
            "1662",
            "1209",
            "1209",
            "1315",
            "1253",
            "1850",
            "1813",
            "1419",
            "1567",
            "1785",
            "1947",
            "1520",
            "1918",
            "1430",
            "1903",
            "1757",
            "1586",
            "1202",
            "1847",
            "1224",
            "1561",
            "1316",
            "1302",
            "1774",
            "1508",
            "1914",
            "1928"
        };
    }
}