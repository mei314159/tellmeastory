
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using TellMe.Core.Contracts.DTO;
using System.Collections.Generic;

namespace TellMe.Core.Types.DataServices.Remote
{
    public class BaseDataService
    {
        public async Task<Result<T>> GetAsync<T>(string uri)
        {
            var result = await this.GetAsync(uri, typeof(T));
            return new Result<T>
            {
                Data = (T)result.Data,
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
                    webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.Instance.AuthInfo.AccessToken);
                    var response = await webClient.GetAsync(requestUri).ConfigureAwait(false);
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
                        var refreshTokenResult = await this.RefreshAuthTokenAsync().ConfigureAwait(false);
                        if (refreshTokenResult.IsSuccess)
                        {
                            return await this.GetAsync(uri, resultType, false).ConfigureAwait(false);
                        }

                        return new Result<object>
                        {
                            IsSuccess = false,
                            IsNetworkIssue = false,
                            ErrorMessage = refreshTokenResult.Error?.ErrorMessage ?? refreshTokenResult.ErrorMessage
                        };
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

        public Task<Result<TResult>> SendDataAsync<TResult>(string uri, object data, HttpMethod method, bool anonymously = false)
        {

            var serializedData = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(serializedData, System.Text.Encoding.UTF8, "application/json");

            return SendDataAsync<TResult>(uri, method, stringContent, anonymously);
        }

        public async Task<Result<TResult>> SendDataAsync<TResult>(string uri, HttpMethod method, HttpContent content, bool anonymously = false, bool refreshExpiredToken = true)
        {
            var result = await SendDataAsync<TResult, Dictionary<string, string[]>>(uri, method, content, anonymously, refreshExpiredToken)
                .ConfigureAwait(false);
            if (!result.IsSuccess)
                result.ModelState = result.Error;
            return result;
        }

        public async Task<Result<TResult, TErrorResult>> SendDataAsync<TResult, TErrorResult>(string uri, HttpMethod method, HttpContent content, bool anonymously = false, bool refreshExpiredToken = true)
        {
            var requestUri = new Uri($"{Constants.ApiHost}/api/{uri}");

            using (var webClient = new HttpClient())
            {
                try
                {
                    if (!anonymously)
                    {
                        webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.Instance.AuthInfo.AccessToken);
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
                        var refreshTokenResult = await this.RefreshAuthTokenAsync().ConfigureAwait(false);
                        if (refreshTokenResult.IsSuccess)
                        {
                            return await this.SendDataAsync<TResult, TErrorResult>(uri, method, content, anonymously, false).ConfigureAwait(false);
                        }

                        return new Result<TResult, TErrorResult>
                        {
                            IsSuccess = false,
                            IsNetworkIssue = false,
                            ErrorMessage = refreshTokenResult.Error?.ErrorMessage ?? refreshTokenResult.ErrorMessage
                        };

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
                    App.Instance.LogNetworkException(ex);
                    return new Result<TResult, TErrorResult>
                    {
                        IsSuccess = false,
                        ErrorMessage = "No internet connection."
                    };
                }
                catch (Exception ex)
                {
                    App.Instance.LogNetworkException(ex);
                    return new Result<TResult, TErrorResult>
                    {
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    };
                }
            }
        }

        private async Task<Result<AuthenticationInfoDTO, AuthenticationErrorDto>> RefreshAuthTokenAsync()
        {
            if (App.Instance.AuthInfo != null)
            {
                var data = new Dictionary<string, string>();
                data.Add("grant_type", "refresh_token");
                data.Add("refresh_token", App.Instance.AuthInfo.RefreshToken);
                data.Add("client_id", "ios_app");
                var result = await this.SendDataAsync<AuthenticationInfoDTO, AuthenticationErrorDto>("token/auth", HttpMethod.Post, new FormUrlEncodedContent(data), true)
                                   .ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    App.Instance.AuthInfo = result.Data;
                }
                else if (result.Error.Code == "905" || result.Error.Code == "906")
                {
                    App.Instance.Authenticate();
                }

                return result;
            }

            return new Result<AuthenticationInfoDTO, AuthenticationErrorDto>
            {
                IsSuccess = false,
                ErrorMessage = "You're not authenticated"
            };
        }
    }
}