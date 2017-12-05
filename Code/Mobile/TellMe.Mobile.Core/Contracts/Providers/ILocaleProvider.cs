using System.Collections.Generic;

namespace TellMe.Mobile.Core.Contracts.Providers
{
    public interface ILocaleProvider
    {
        string GetCountryCode();

        IReadOnlyDictionary<string, string> GetCountryNames();
        string GetCountryName(string countryCode);
    }
}