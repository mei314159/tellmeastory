using Newtonsoft.Json;

namespace TellMe.Core.Dto
{

    public class ErrorDTO
    {
        [JsonProperty(PropertyName = "error")]
        public virtual string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public virtual string ErrorMessage { get; set; }
    }


    public class AuthenticationErrorDto : ErrorDTO
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

		[JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}