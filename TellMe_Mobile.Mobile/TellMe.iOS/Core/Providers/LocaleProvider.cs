using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using TellMe.Core;
using TellMe.Core.Contracts.Providers;

namespace TellMe.iOS.Core.Providers
{
    public class LocaleProvider : ILocaleProvider
    {
        public string GetCountryCode()
        {
            return NSLocale.CurrentLocale.CountryCode;
        }

        public string GetCountryName(string countryCode)
        {
            return NSLocale.CurrentLocale.GetCountryCodeDisplayName(countryCode);
        }

        public IReadOnlyDictionary<string, string> GetCountryNames()
        {
            var countries = PhoneCodes.CountryCodes.ToDictionary(x => x.Key, x => NSLocale.CurrentLocale.GetCountryCodeDisplayName(x.Key));
            return countries;
        }
    }
}
