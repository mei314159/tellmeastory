using Newtonsoft.Json;

namespace TellMe.Mobile.Core.Contracts.DTO
{
    public class AuthenticationErrorDto : ErrorDTO
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}