using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        OsType OsType { get; }

        string AppVersion { get; }

        T Get<T>(string key);

        bool GetBool(string key);

        void Set<T>(string key, T value) where T : class;

        void SetBool(string key, bool value);
    }
}