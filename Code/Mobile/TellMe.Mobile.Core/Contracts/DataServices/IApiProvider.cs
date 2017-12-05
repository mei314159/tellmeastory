using System;
using System.Net.Http;
using System.Threading.Tasks;
using TellMe.Mobile.Core.Contracts.DTO;

namespace TellMe.Mobile.Core.Contracts.DataServices
{
    public interface IApiProvider
    {
        Task<Result<T>> GetAsync<T>(string uri);
        Task<Result<object>> GetAsync(string uri, Type resultType, bool refreshExpiredToken = true);
        Task<Result<TResult>> PutAsync<TResult>(string uri, object data);
        Task<Result<TResult>> DeleteAsync<TResult>(string uri, object data);
        Task<Result<TResult>> PostAsync<TResult>(string uri, object data, bool anonymously = false);

        Task<Result<TResult>> SendDataAsync<TResult>(string uri, object data, HttpMethod method,
            bool anonymously = false);

        Task<Result<TResult>> SendDataAsync<TResult>(string uri, HttpMethod method, HttpContent content,
            bool anonymously = false, bool refreshExpiredToken = true);

        Task<Result<TResult, TErrorResult>> SendDataAsync<TResult, TErrorResult>(string uri, HttpMethod method,
            HttpContent content, bool anonymously = false, bool refreshExpiredToken = true);
    }
}