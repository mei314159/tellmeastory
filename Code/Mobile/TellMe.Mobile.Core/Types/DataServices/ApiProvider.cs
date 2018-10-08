using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using TellMe.Mobile.Core.Contracts;
using TellMe.Mobile.Core.Contracts.DataServices;
using TellMe.Mobile.Core.Contracts.DataServices.Local;
using TellMe.Mobile.Core.Contracts.DTO;
using TellMe.Mobile.Core.Types.Tasks;

namespace TellMe.Mobile.Core.Types.DataServices
{
    public class ApiProvider : IApiProvider
    {
        private readonly ILocalAccountService _localLocalAccountService;
        private readonly IRouter _router;
        private static readonly TaskSynchronizationScope Lock = new TaskSynchronizationScope();
        public ApiProvider(ILocalAccountService localLocalAccountService, IRouter router)
        {
            _localLocalAccountService = localLocalAccountService;
            _router = router;
        }

        public async Task<Result<T>> GetAsync<T>(string uri)
        {
            var result = await this.GetAsync(uri, typeof(T));
            return new Result<T>
            {
                Data = (T) result.Data,
                ErrorMessage = result.ErrorMessage,
                IsSuccess = result.IsSuccess,
                ModelState = result.ModelState,
                IsNetworkIssue = result.IsNetworkIssue
            };
        }

        public async Task<Result<object>> GetAsync(string uri, Type resultType, bool refreshExpiredToken = true)
        {
            var requestUri = new Uri($"{Constants.ApiHost}/api/{uri}");

            using (var webClient = new HttpClient())
            {
                try
                {
                    var accessToken = await this.GetAccessTokenAsync().ConfigureAwait(false);
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        accessToken);
                    var response = await webClient.GetAsync(requestUri).ConfigureAwait(false);
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debug.WriteLine(responseString);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = JsonConvert.DeserializeObject(responseString, resultType);
                        return new Result<object>
                        {
                            IsSuccess = true,
                            Data = data,
                            ErrorMessage = null
                        };
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized && refreshExpiredToken)
                    {
                        return await this.GetAsync(uri, resultType, false).ConfigureAwait(false);
                    }

                    var errorResponse = JsonConvert.DeserializeObject<ErrorDTO>(responseString);
                    return new Result<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = errorResponse.ErrorMessage
                    };
                }
                catch (WebException ex)
                {
                    Crashes.TrackError(ex);
                    App.Instance.LogNetworkException(ex);
                    return new Result<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message,
                        IsNetworkIssue = true
                    };
                }
                catch (HttpRequestException ex)
                {
                    Crashes.TrackError(ex);
                    App.Instance.LogNetworkException(ex);
                    return new Result<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = "No internet connection.",
                        IsNetworkIssue = true
                    };
                }

                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    App.Instance.LogNetworkException(ex);
                    return new Result<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        public Task<Result<TResult>> PutAsync<TResult>(string uri, object data)
        {
            return SendDataAsync<TResult>(uri, data, HttpMethod.Put);
        }

        public Task<Result<TResult>> DeleteAsync<TResult>(string uri, object data)
        {
            return SendDataAsync<TResult>(uri, data, HttpMethod.Delete);
        }

        public Task<Result<TResult>> PostAsync<TResult>(string uri, object data, bool anonymously = false)
        {
            return SendDataAsync<TResult>(uri, data, HttpMethod.Post, anonymously);
        }

        public Task<Result<TResult>> SendDataAsync<TResult>(string uri, object data, HttpMethod method,
            bool anonymously = false)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(serializedData, System.Text.Encoding.UTF8, "application/json");

            return SendDataAsync<TResult>(uri, method, stringContent, anonymously);
        }

        public async Task<Result<TResult>> SendDataAsync<TResult>(string uri, HttpMethod method, HttpContent content,
            bool anonymously = false, bool refreshExpiredToken = true)
        {
            var result =
                await SendDataAsync<TResult, Dictionary<string, string[]>>(uri, method, content, anonymously,
                        refreshExpiredToken)
                    .ConfigureAwait(false);
            if (!result.IsSuccess)
                result.ModelState = result.Error;
            return result;
        }

        public async Task<Result<TResult, TErrorResult>> SendDataAsync<TResult, TErrorResult>(string uri,
            HttpMethod method, HttpContent content, bool anonymously = false, bool refreshExpiredToken = true)
        {
            var requestUri = new Uri($"{Constants.ApiHost}/api/{uri}");

            using (var webClient = new HttpClient())
            {
                try
                {
                    if (!anonymously)
                    {
                        var accessToken = await this.GetAccessTokenAsync().ConfigureAwait(false);
                        webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                            accessToken);
                    }
                    else
                    {
                        webClient.DefaultRequestHeaders.Remove("Authorization");
                    }

                    HttpResponseMessage response;
                    if (method == HttpMethod.Put)
                        response = await webClient.PutAsync(requestUri, content).ConfigureAwait(false);
                    else if (method == HttpMethod.Post)
                        response = await webClient.PostAsync(requestUri, content);
                    else if (method == HttpMethod.Delete)
                        response = await webClient.DeleteAsync(requestUri).ConfigureAwait(false);
                    else
                    {
                        throw new ArgumentException("HttpMethod is not supported", nameof(method));
                    }

                    string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debug.WriteLine(responseString);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var messageDto = JsonConvert.DeserializeObject<TResult>(responseString);
                        return new Result<TResult, TErrorResult>
                        {
                            IsSuccess = true,
                            Data = messageDto,
                            ErrorMessage = null
                        };
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized && !anonymously && refreshExpiredToken)
                    {
                        return await this.SendDataAsync<TResult, TErrorResult>(uri, method, content, false, false)
                            .ConfigureAwait(false);
                    }

                    var error = JsonConvert.DeserializeObject<TErrorResult>(responseString);
                    return new Result<TResult, TErrorResult>
                    {
                        IsSuccess = false,
                        Error = error,
                        ErrorMessage = response.ReasonPhrase
                    };
                }
                catch (HttpRequestException ex)
                {
                    Crashes.TrackError(ex);
                    App.Instance.LogNetworkException(ex);
                    return new Result<TResult, TErrorResult>
                    {
                        IsSuccess = false,
                        ErrorMessage = "No internet connection."
                    };
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    App.Instance.LogNetworkException(ex);
                    return new Result<TResult, TErrorResult>
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var authInfo = _localLocalAccountService.GetAuthInfo();
            if (authInfo.Expired)
            {
                await Lock.RunAsync(async () =>
                {
                    authInfo = _localLocalAccountService.GetAuthInfo();
                    if (authInfo.Expired)
                    {
                        authInfo = await RefreshAuthTokenAsync().ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
            }
            
            return authInfo.AccessToken;
        }

        private async Task<AuthenticationInfoDTO> RefreshAuthTokenAsync()
        {
            var authInfo = _localLocalAccountService.GetAuthInfo();
            if (authInfo == null)
            {
                throw new Exception("You're not authenticated");
            }

            var data = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", authInfo.RefreshToken},
                {"client_id", "ios_app"}
            };
            var result = await this.SendDataAsync<AuthenticationInfoDTO, AuthenticationErrorDto>("token/auth",
                    HttpMethod.Post, new FormUrlEncodedContent(data), true)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                _localLocalAccountService.SaveAuthInfo(result.Data);
                return result.Data;
            }
            
            if (result.Error.Code == "905" || result.Error.Code == "906")
            {
                _localLocalAccountService.SaveAuthInfo(null);
                _router.SwapToAuth();
            }
            
            throw new Exception(result.ErrorMessage);
        }
    }
}