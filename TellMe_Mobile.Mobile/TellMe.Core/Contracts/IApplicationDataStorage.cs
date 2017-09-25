using TellMe.Core.Contracts.DTO;

namespace TellMe.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoDTO AuthInfo { get; set; }
        T Get<T>(string key);
        void Set<T>(string key, T result) where T : class;
    }
}
